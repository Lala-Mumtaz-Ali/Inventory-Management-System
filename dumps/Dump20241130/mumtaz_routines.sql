-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: mumtaz
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Dumping events for database 'mumtaz'
--

--
-- Dumping routines for database 'mumtaz'
--
/*!50003 DROP PROCEDURE IF EXISTS `DupProcessAndDispatchOrderItem` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DupProcessAndDispatchOrderItem`(
    IN p_order_item_id INT,
    IN p_warehouse_id INT
)
BEGIN
    DECLARE total_quantity INT;
    DECLARE available_quantity INT DEFAULT 0;  -- Default to 0 to handle NULL case
    DECLARE remaining_quantity INT;
    DECLARE lot_id INT;
    DECLARE lot_quantity INT;
    DECLARE product_id INT;
    DECLARE done INT DEFAULT 0;  -- Variable to handle the cursor completion status
    DECLARE lot_cursor CURSOR FOR 
        SELECT pl.Id, pl.Quantity 
        FROM Product_Lot pl
        WHERE pl.Product_id = product_id
        AND pl.WareHouse_Id = p_warehouse_id
        ORDER BY pl.Manufacturing_date;  -- Process the oldest lots first
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    -- Get the product and quantity for the order item
    SELECT oi.Product_Id, oi.Quantity
    INTO product_id, total_quantity
    FROM OrderItems oi
    WHERE oi.Id = p_order_item_id;

    -- Get the total available quantity of the product in the given warehouse
    SELECT COALESCE(SUM(pl.Quantity), 0)  -- Use COALESCE to return 0 if no rows are found
    INTO available_quantity
    FROM Product_Lot pl
    WHERE pl.Product_id = product_id
    AND pl.WareHouse_Id = p_warehouse_id;

    -- Check if the available quantity is enough to fulfill the order item
    IF available_quantity < total_quantity THEN
        -- If not enough stock, throw an error
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Not enough stock to process the order item.';
    END IF;

    SET remaining_quantity = total_quantity;

    -- Declare the cursor to process the lots
   
    -- Declare a continue handler for when no more rows are available in the cursor
   

    OPEN lot_cursor;

    read_loop: LOOP
        FETCH lot_cursor INTO lot_id, lot_quantity;

        -- If there are no rows left to fetch (done = 1), exit the loop
        IF done THEN
            LEAVE read_loop;
        END IF;

        -- Skip lots with 0 quantity (no need to process them)
        IF lot_quantity = 0 THEN
            ITERATE read_loop;  -- Skip this iteration and move to the next lot
        END IF;

        -- Process the lot, deduct the required quantity from the lot
        IF lot_quantity >= remaining_quantity THEN
            -- Deduct the remaining quantity from this lot
            UPDATE Product_Lot 
            SET Quantity = Quantity - remaining_quantity
            WHERE Id = lot_id;

            SET remaining_quantity = 0;  -- Fulfilled the order item
        ELSE
            -- Deduct the entire quantity from this lot
            UPDATE Product_Lot 
            SET Quantity = 0
            WHERE Id = lot_id;

            SET remaining_quantity = remaining_quantity - lot_quantity;
        END IF;
    END LOOP;

    CLOSE lot_cursor;

    -- Mark the order as dispatched
    
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `FinalProcessAndDispatchOrderItem` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `FinalProcessAndDispatchOrderItem`(
    IN p_order_item_id INT,
    IN p_warehouse_id INT
)
BEGIN
    DECLARE total_quantity INT;
    DECLARE available_quantity INT DEFAULT 0;
    DECLARE remaining_quantity INT;
    DECLARE lot_id INT;
    DECLARE lot_quantity INT;
    DECLARE product_id INT;
    DECLARE reorder_level INT;
    DECLARE category_id INT;
    DECLARE purchase_quantity INT;
    DECLARE supplier_id INT;
    DECLARE done INT DEFAULT 0;
    DECLARE product_price INT;

    DECLARE lot_cursor CURSOR FOR 
        SELECT pl.Id, pl.Quantity 
        FROM Product_Lot pl
        WHERE pl.Product_id = product_id
        AND pl.WareHouse_Id = p_warehouse_id
        ORDER BY pl.Manufacturing_date;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    -- Fetch product_id and total_quantity from OrderItems
    SELECT oi.Product_Id, oi.Quantity
    INTO product_id, total_quantity
    FROM OrderItems oi
    WHERE oi.Id = p_order_item_id;

    -- Fetch available quantity from Product_Lot
    SELECT COALESCE(SUM(pl.Quantity), 0)
    INTO available_quantity
    FROM Product_Lot pl
    WHERE pl.Product_id = product_id
    AND pl.WareHouse_Id = p_warehouse_id;

    -- Check if stock is sufficient
    IF available_quantity < total_quantity THEN
       
            SIGNAL SQLSTATE '45000' 
            SET MESSAGE_TEXT = 'Not enough stock.';
        
           
    END IF;

    SET remaining_quantity = total_quantity;

    -- Open the cursor to process the lots
    OPEN lot_cursor;

    read_loop: LOOP
        FETCH lot_cursor INTO lot_id, lot_quantity;

        IF done THEN
            LEAVE read_loop;
        END IF;

        IF lot_quantity = 0 THEN
            ITERATE read_loop;
        END IF;

        -- Process the lot, deduct the required quantity
        IF lot_quantity >= remaining_quantity THEN
            UPDATE Product_Lot 
            SET Quantity = Quantity - remaining_quantity
            WHERE Id = lot_id;

            SET remaining_quantity = 0;
        ELSE
            UPDATE Product_Lot 
            SET Quantity = 0
            WHERE Id = lot_id;

            SET remaining_quantity = remaining_quantity - lot_quantity;
        END IF;

        DELETE FROM Product_Lot 
        WHERE Id = lot_id AND Quantity = 0;
    END LOOP;

    CLOSE lot_cursor;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTransferableWarehouses` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTransferableWarehouses`(IN lot_id INT)
BEGIN
    DECLARE product_id INT;
    DECLARE product_quantity INT;
    DECLARE warehouse_id INT;

    -- Get the product_id, quantity, and current warehouse_id of the product lot
    SELECT Product_id, Quantity, WareHouse_Id
    INTO product_id, product_quantity, warehouse_id
    FROM Product_Lot
    WHERE Id = lot_id;

    -- Select the transferable warehouses (excluding the current warehouse)
    SELECT w.Id, w.Name, w.Location, w.PerProductCapacity
    FROM WareHouse w
    WHERE w.Id != warehouse_id  -- Exclude the current warehouse
    AND w.PerProductCapacity >= (
        -- Check if adding the lot quantity to the current product's total in the warehouse exceeds capacity
	
            SELECT SUM(pl.Quantity)
            FROM Product_Lot pl
            WHERE pl.WareHouse_Id = w.Id AND pl.Product_id = product_id
            );
        
    
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `MyProcessAndDispatchOrderItem` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `MyProcessAndDispatchOrderItem`(
    IN p_order_item_id INT,
    IN p_warehouse_id INT
)
BEGIN
    DECLARE total_quantity INT;
    DECLARE available_quantity INT DEFAULT 0;
    DECLARE remaining_quantity INT;
    DECLARE lot_id INT;
    DECLARE lot_quantity INT;
    DECLARE product_id INT;
    DECLARE reorder_level INT;
    DECLARE category_id INT;
    DECLARE purchase_quantity INT;
    DECLARE supplier_id INT;
    DECLARE done INT DEFAULT 0;
    DECLARE product_price INT;

    DECLARE lot_cursor CURSOR FOR 
        SELECT pl.Id, pl.Quantity 
        FROM Product_Lot pl
        WHERE pl.Product_id = product_id
        AND pl.WareHouse_Id = p_warehouse_id
        ORDER BY pl.Manufacturing_date;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    -- Get the product and quantity for the order item
    SELECT oi.Product_Id, oi.Quantity
    INTO product_id, total_quantity
    FROM OrderItems oi
    WHERE oi.Id = p_order_item_id;

    -- Get the total available quantity of the product in the given warehouse
    SELECT COALESCE(SUM(pl.Quantity), 0)
    INTO available_quantity
    FROM Product_Lot pl
    WHERE pl.Product_id = product_id
    AND pl.WareHouse_Id = p_warehouse_id;

    -- Get the low stock threshold, category_id, and current price for the product
    SELECT p.Low_Stock_Threshold, p.Category_Id, p.Current_Price
    INTO reorder_level, category_id, product_price
    FROM Product p
    WHERE p.Id = product_id;

    -- Calculate the purchase quantity (twice the low stock threshold)
    SET purchase_quantity = reorder_level * 2;

    -- Get the supplier_id based on the category_id
    SELECT s.Id
    INTO supplier_id
    FROM Supplier s
    WHERE s.Category_Id = category_id;
    
    
    
      -- Assuming we take the first supplier in the category

    -- Check if the available quantity is enough to fulfill the order item
    IF available_quantity < total_quantity THEN
        -- Place a purchase order if stock is low
        INSERT INTO PurchaseOrders (
            Product_Id, WareHouse_Id, Order_Date, Delivery_Date, Quantity, TotalPrice, Is_Recieved, Supplier_Id
        ) VALUES (
            product_id,
            p_warehouse_id,
            NOW(),  -- Order date
            DATE_ADD(NOW(), INTERVAL 7 DAY),  -- Delivery date (example: 7 days from now)
            purchase_quantity,  -- Purchase quantity (twice the low stock threshold)
            purchase_quantity * product_price, -- Total price based on current price from the Product table
            0,  -- Not received yet
            supplier_id
        );

        -- Throw an error to notify about low stock
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Not enough stock. A purchase order has been placed.';
    END IF;

    SET remaining_quantity = total_quantity;

    -- Open the cursor to process the lots
    OPEN lot_cursor;

    read_loop: LOOP
        FETCH lot_cursor INTO lot_id, lot_quantity;

        IF done THEN
            LEAVE read_loop;
        END IF;

        IF lot_quantity = 0 THEN
            ITERATE read_loop;
        END IF;

        -- Process the lot, deduct the required quantity
        IF lot_quantity >= remaining_quantity THEN
            UPDATE Product_Lot 
            SET Quantity = Quantity - remaining_quantity
            WHERE Id = lot_id;

            SET remaining_quantity = 0;
        ELSE
            UPDATE Product_Lot 
            SET Quantity = 0
            WHERE Id = lot_id;

            SET remaining_quantity = remaining_quantity - lot_quantity;
        END IF;

        DELETE FROM Product_Lot 
        WHERE Id = lot_id AND Quantity = 0;
    END LOOP;

    CLOSE lot_cursor;

    -- Optionally mark the order as dispatched
    
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `NewProcessAndDispatchOrderItem` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `NewProcessAndDispatchOrderItem`(
    IN p_order_item_id INT,
    IN p_warehouse_id INT
)
BEGIN
    DECLARE total_quantity INT;
    DECLARE available_quantity INT DEFAULT 0;  -- Default to 0 to handle NULL case
    DECLARE remaining_quantity INT;
    DECLARE lot_id INT;
    DECLARE lot_quantity INT;
    DECLARE product_id INT;
    DECLARE done INT DEFAULT 0;  -- Variable to handle the cursor completion status
    DECLARE lot_cursor CURSOR FOR 
        SELECT pl.Id, pl.Quantity 
        FROM Product_Lot pl
        WHERE pl.Product_id = product_id
        AND pl.WareHouse_Id = p_warehouse_id
        ORDER BY pl.Manufacturing_date;  -- Process the oldest lots first
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    
    -- Get the product and quantity for the order item
    SELECT oi.Product_Id, oi.Quantity
    INTO product_id, total_quantity
    FROM OrderItems oi
    WHERE oi.Id = p_order_item_id;

    -- Get the total available quantity of the product in the given warehouse
    SELECT COALESCE(SUM(pl.Quantity), 0)  -- Use COALESCE to return 0 if no rows are found
    INTO available_quantity
    FROM Product_Lot pl
    WHERE pl.Product_id = product_id
    AND pl.WareHouse_Id = p_warehouse_id;

    -- Check if the available quantity is enough to fulfill the order item
    IF available_quantity < total_quantity THEN
        -- If not enough stock, throw an error
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Not enough stock to process the order item.';
    END IF;

    SET remaining_quantity = total_quantity;

    -- Declare the cursor to process the lots
    OPEN lot_cursor;

    read_loop: LOOP
        FETCH lot_cursor INTO lot_id, lot_quantity;

        -- If there are no rows left to fetch (done = 1), exit the loop
        IF done THEN
            LEAVE read_loop;
        END IF;

        -- Skip lots with 0 quantity (no need to process them)
        IF lot_quantity = 0 THEN
            ITERATE read_loop;  -- Skip this iteration and move to the next lot
        END IF;

        -- Process the lot, deduct the required quantity from the lot
        IF lot_quantity >= remaining_quantity THEN
            -- Deduct the remaining quantity from this lot
            UPDATE Product_Lot 
            SET Quantity = Quantity - remaining_quantity
            WHERE Id = lot_id;

            SET remaining_quantity = 0;  -- Fulfilled the order item
        ELSE
            -- Deduct the entire quantity from this lot
            UPDATE Product_Lot 
            SET Quantity = 0
            WHERE Id = lot_id;

            SET remaining_quantity = remaining_quantity - lot_quantity;
        END IF;

        -- Delete the lot if its quantity has become zero
        DELETE FROM Product_Lot WHERE Id = lot_id AND Quantity = 0;

    END LOOP;

    CLOSE lot_cursor;

    -- Mark the order as dispatched
    

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ProcessAndDispatchOrderItem` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ProcessAndDispatchOrderItem`(
    IN p_order_item_id INT,
    IN p_warehouse_id INT
)
BEGIN
    DECLARE total_quantity INT;
    DECLARE available_quantity INT DEFAULT 0;  -- Default to 0 to handle NULL case
    DECLARE remaining_quantity INT;
    DECLARE lot_id INT;
    DECLARE lot_quantity INT;
    DECLARE product_id INT;
    DECLARE done INT DEFAULT 0;  -- Variable to handle the cursor completion status
    DECLARE lot_cursor CURSOR FOR 
        SELECT pl.Id, pl.Quantity 
        FROM Product_Lot pl
        WHERE pl.Product_id = product_id
        AND pl.WareHouse_Id = p_warehouse_id
        ORDER BY pl.Manufacturing_date;  -- Process the oldest lots first
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    -- Get the product and quantity for the order item
    SELECT oi.Product_Id, oi.Quantity
    INTO product_id, total_quantity
    FROM OrderItems oi
    WHERE oi.Id = p_order_item_id;

    -- Get the total available quantity of the product in the given warehouse
    SELECT COALESCE(SUM(pl.Quantity), 0)  -- Use COALESCE to return 0 if no rows are found
    INTO available_quantity
    FROM Product_Lot pl
    WHERE pl.Product_id = product_id
    AND pl.WareHouse_Id = p_warehouse_id;

    -- Check if the available quantity is enough to fulfill the order item
    IF available_quantity < total_quantity THEN
        -- If not enough stock, throw an error
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Not enough stock to process the order item.';
    END IF;

    SET remaining_quantity = total_quantity;

    -- Declare the cursor to process the lots
   
    -- Declare a continue handler for when no more rows are available in the cursor
   

    OPEN lot_cursor;

    read_loop: LOOP
        FETCH lot_cursor INTO lot_id, lot_quantity;

        -- If there are no rows left to fetch (done = 1), exit the loop
        IF done THEN
            LEAVE read_loop;
        END IF;

        -- Skip lots with 0 quantity (no need to process them)
        IF lot_quantity = 0 THEN
            ITERATE read_loop;  -- Skip this iteration and move to the next lot
        END IF;

        -- Process the lot, deduct the required quantity from the lot
        IF lot_quantity >= remaining_quantity THEN
            -- Deduct the remaining quantity from this lot
            UPDATE Product_Lot 
            SET Quantity = Quantity - remaining_quantity
            WHERE Id = lot_id;

            SET remaining_quantity = 0;  -- Fulfilled the order item
        ELSE
            -- Deduct the entire quantity from this lot
            UPDATE Product_Lot 
            SET Quantity = 0
            WHERE Id = lot_id;

            SET remaining_quantity = remaining_quantity - lot_quantity;
        END IF;
    END LOOP;

    CLOSE lot_cursor;

    -- Mark the order as dispatched
    UPDATE Orders
    SET Is_Dispatched = 'Dispatched'
    WHERE Id = (SELECT Order_Id FROM OrderItems WHERE Id = p_order_item_id);

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ReorderFailedOrderItems` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ReorderFailedOrderItems`(
    IN p_order_id INT,
    IN p_warehouse_id INT
)
BEGIN
    DECLARE done INT DEFAULT 0;
    DECLARE order_item_id INT;
    DECLARE product_id INT;
    DECLARE total_quantity INT;
    DECLARE available_quantity INT DEFAULT 0;
    DECLARE reorder_level INT;
    DECLARE category_id INT;
    DECLARE purchase_quantity INT;
    DECLARE supplier_id INT;
    DECLARE product_price INT;

    DECLARE lot_cursor CURSOR FOR 
        SELECT oi.Id, oi.Product_Id, oi.Quantity
        FROM OrderItems oi
        WHERE oi.Order_Id = p_order_id;

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    -- Open cursor to process each order item in the failed order
    OPEN lot_cursor;

    read_loop: LOOP
        FETCH lot_cursor INTO order_item_id, product_id, total_quantity;

        IF done THEN
            LEAVE read_loop;
        END IF;

        -- Check the available quantity in the warehouse
        SELECT COALESCE(SUM(pl.Quantity), 0)
        INTO available_quantity
        FROM Product_Lot pl
        WHERE pl.Product_id = product_id
        AND pl.WareHouse_Id = p_warehouse_id;

        -- If available quantity is less than the ordered quantity, reorder
        IF available_quantity < total_quantity THEN
            -- Get reorder level and product details
            SELECT p.Low_Stock_Threshold, p.Category_Id, p.Current_Price
            INTO reorder_level, category_id, product_price
            FROM Product p
            WHERE p.Id = product_id;

            -- Calculate the purchase quantity (twice the reorder level)
            SET purchase_quantity = reorder_level * 2;

            -- Get the supplier for the product category
            SELECT s.Id
            INTO supplier_id
            FROM Supplier s
            WHERE s.category_id = category_id;

            -- Check if a purchase order already exists for this product in the warehouse
            IF NOT EXISTS (
                SELECT 1
                FROM PurchaseOrders po
                WHERE po.Product_Id = product_id
                AND po.WareHouse_Id = p_warehouse_id
                AND po.Is_Received = 0
            ) THEN
                -- Create a new purchase order
                INSERT INTO PurchaseOrders (
                    Product_Id, WareHouse_Id, Order_Date, Delivery_Date, Quantity, TotalPrice, Is_Received, Supplier_Id
                ) VALUES (
                    product_id,
                    p_warehouse_id,
                    NOW(),  -- Order date
                    DATE_ADD(NOW(), INTERVAL 7 DAY),  -- Delivery date (7 days from now)
                    purchase_quantity,  -- Purchase quantity
                    purchase_quantity * product_price, -- Total price
                    0,  -- Not received yet
                    supplier_id
                );
                 commit;
                -- Log the creation of the purchase order
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Purchase order created for insufficient stock.';
            ELSE
                -- Log that a purchase order already exists
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Purchase order already pending for this product and warehouse.';
            END IF;
        END IF;
    END LOOP;

    -- Close the cursor after processing all order items
    CLOSE lot_cursor;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-11-30  4:13:57
