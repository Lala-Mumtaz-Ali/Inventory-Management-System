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
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Category_id` int NOT NULL,
  `Current_Price` int NOT NULL,
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Low_Stock_Threshold` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `AK_Product_Name` (`Name`),
  KEY `FK_Product_Category_Category_id` (`Category_id`),
  CONSTRAINT `FK_Product_Category_Category_id` FOREIGN KEY (`Category_id`) REFERENCES `category` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_Price` CHECK ((`Current_Price` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
INSERT INTO `product` VALUES (1,'RunFalcon',1,100,'Running shoes designed for high performance and comfort. Ideal for marathon training.',20),(2,'SportPro',1,120,'Sports shoes engineered for agility, perfect for indoor and outdoor activities.',15),(3,'LoaferGents',1,80,'Men’s loafers, stylish and comfortable, suitable for both casual and semi-formal occasions.',5),(4,'CasualStride',1,65,'Casual shoes for everyday wear, with a soft sole for all-day comfort.',8),(5,'GolfMaster',1,150,'Golf shoes with a stable grip and waterproof design, ensuring durability on the green.',20),(6,'RunningLite',1,90,'Lightweight running shoes with breathable material for speed and comfort.',20),(7,'LoaferChic',1,75,'Women’s loafers designed for both elegance and comfort, perfect for office and casual wear.',20),(8,'TrailBlaze',1,110,'Trail running shoes built for rugged outdoor terrains, providing superior grip and protection.',9),(9,'SportsElite',1,130,'High-performance sports shoes with extra cushion support, ideal for intense workouts and competitions.',7),(10,'SlipOnFlex',1,50,'Slip-on casual shoes with flexible soles for ease of wear and comfort.',10),(12,'BusinessElite',3,65,'Updated description',10),(13,'CasualFit',3,40,'Comfortable casual shirt',20),(14,'PlaidVibe',3,50,'Stylish plaid shirt',12),(15,'DenimStyle',3,55,'Rugged denim shirt',8),(16,'LinenBreeze',3,70,'Relaxed linen shirt',18),(17,'DressMax',3,80,'Elegant dress shirt',5),(18,'HawaiianTropics',3,35,'Colorful Hawaiian shirt',25),(19,'TechFlex',3,60,'Moisture-wicking tech shirt',10),(20,'V-NeckComfort',3,30,'Soft V-neck shirt',30),(21,'SlimFitDenim',4,50,'Trendy slim-fit jeans',20),(22,'ChinoFlex',4,45,'Comfortable chino pants',15),(23,'CargoPro',4,55,'Durable cargo pants',12),(24,'SportFlex',4,60,'Active wear pants',10),(25,'ClassicKhaki',4,40,'Classic khaki pants',18),(26,'SlimStretch',4,52,'Stretchable slim pants',22),(27,'BootCutStyle',4,48,'Stylish boot cut jeans',14),(28,'JoggerFit',4,35,'Casual jogger pants',30),(29,'TailoredFit',4,65,'Tailored formal pants',8),(30,'AthleticFit',4,58,'Athletic fit workout pants',25),(31,'LeatherWallet',5,30,'Genuine leather wallet',20),(32,'Sunglasses',5,25,'UV-protected sunglasses',40),(33,'Wristwatch',5,80,'Stylish wristwatch',30),(34,'MessengerBag',5,55,'Durable messenger bag',25),(35,'Backpack',5,45,'Spacious and stylish backpack',25),(36,'Cap',5,15,'Casual baseball cap',20),(37,'Necklace',5,40,'Elegant silver necklace',20),(38,'Bracelet',5,22,'Fashionable leather bracelet',30),(39,'Belt',5,28,'Premium leather belt',35),(40,'Scarf',5,20,'Warm and cozy scarf',30),(41,'Tudor Watch',5,100000,'Your luxury is our goal.',10);
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `trg_after_insert_product` AFTER INSERT ON `product` FOR EACH ROW BEGIN
    -- Insert a PurchaseOrder for each warehouse
    INSERT INTO PurchaseOrders (Product_Id, WareHouse_Id, Order_Date, Quantity, TotalPrice, Supplier_Id,Delivery_date,Is_Received)
    SELECT 
        NEW.Id,
        w.Id,             
        CURDATE(),           
        w.PerProductCapacity, 
        0,                    
        s.Id ,
      DATE_ADD(CURDATE(), INTERVAL 5 DAY),
      0

    FROM WareHouse w
    LEFT JOIN Supplier s ON s.Category_id = NEW.Category_id; -- Match Supplier with the Category_id
END */;;
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
