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
-- Table structure for table `purchaseorders`
--

DROP TABLE IF EXISTS `purchaseorders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchaseorders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Product_Id` int DEFAULT NULL,
  `WareHouse_Id` int DEFAULT NULL,
  `Order_Date` date DEFAULT NULL,
  `Quantity` int DEFAULT NULL,
  `TotalPrice` int DEFAULT NULL,
  `Supplier_Id` int DEFAULT NULL,
  `Delivery_Date` date DEFAULT NULL,
  `Is_Received` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Product_Id` (`Product_Id`),
  KEY `Supplier_Id` (`Supplier_Id`),
  CONSTRAINT `purchaseorders_ibfk_2` FOREIGN KEY (`Product_Id`) REFERENCES `product` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `purchaseorders_ibfk_3` FOREIGN KEY (`Supplier_Id`) REFERENCES `supplier` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchaseorders`
--

LOCK TABLES `purchaseorders` WRITE;
/*!40000 ALTER TABLE `purchaseorders` DISABLE KEYS */;
INSERT INTO `purchaseorders` VALUES (1,18,1001,'2024-11-30',50,1750,2002,'2024-12-07',0),(2,3,1001,'2024-11-30',150,12000,2000,'2024-10-05',1),(3,37,1004,'2024-11-30',260,10400,2004,'2024-10-05',1),(4,37,1003,'2024-11-30',250,10000,2004,'2024-10-05',1),(5,37,1002,'2024-11-30',200,8000,2004,'2024-10-05',1),(6,37,1001,'2024-11-30',150,6000,2004,'2024-10-05',1),(7,38,1004,'2024-11-30',260,5720,2004,'2024-10-05',1),(8,38,1003,'2024-11-30',250,5500,2004,'2024-10-05',1),(9,38,1002,'2024-11-30',200,4400,2004,'2024-10-05',1),(10,38,1001,'2024-11-30',150,3300,2004,'2024-10-05',1),(11,39,1004,'2024-11-30',260,7280,2004,'2024-10-05',1),(12,39,1003,'2024-11-30',250,7000,2004,'2024-10-05',1),(13,39,1002,'2024-11-30',200,5600,2004,'2024-10-05',1),(14,39,1001,'2024-11-30',150,4200,2004,'2024-10-05',1),(15,40,1004,'2024-11-30',260,5200,2004,'2024-10-05',1),(16,40,1003,'2024-11-30',250,5000,2004,'2024-10-05',1),(17,40,1002,'2024-11-30',200,4000,2004,'2024-10-05',1),(18,40,1001,'2024-11-30',150,3000,2004,'2024-10-05',1),(19,41,1004,'2024-11-30',260,26000000,2004,'2024-10-05',1),(20,41,1003,'2024-11-30',250,25000000,2004,'2024-10-05',1),(21,41,1002,'2024-11-30',200,20000000,2004,'2024-10-05',1),(22,41,1001,'2024-11-30',150,15000000,2004,'2024-10-05',1);
/*!40000 ALTER TABLE `purchaseorders` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-11-30  4:13:57
