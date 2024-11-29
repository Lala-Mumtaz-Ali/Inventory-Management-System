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
-- Table structure for table `lotmovements`
--

DROP TABLE IF EXISTS `lotmovements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lotmovements` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Source` int NOT NULL,
  `Destination` int NOT NULL,
  `Lot_Id` int NOT NULL,
  `Transaction_date` datetime NOT NULL,
  `Related_Movement` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Source` (`Source`),
  KEY `Destination` (`Destination`),
  KEY `fk_warehouse` (`Related_Movement`),
  KEY `lotmovements_ibfk_4` (`Lot_Id`),
  CONSTRAINT `fk_warehouse` FOREIGN KEY (`Related_Movement`) REFERENCES `lotmovements` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `lotmovements_ibfk_1` FOREIGN KEY (`Source`) REFERENCES `warehouse` (`Id`),
  CONSTRAINT `lotmovements_ibfk_2` FOREIGN KEY (`Destination`) REFERENCES `warehouse` (`Id`),
  CONSTRAINT `lotmovements_ibfk_4` FOREIGN KEY (`Lot_Id`) REFERENCES `product_lot` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lotmovements`
--

LOCK TABLES `lotmovements` WRITE;
/*!40000 ALTER TABLE `lotmovements` DISABLE KEYS */;
INSERT INTO `lotmovements` VALUES (1,1001,1003,10,'2024-11-28 11:10:19',NULL),(2,1003,1002,10,'2024-11-28 11:11:12',1),(3,1002,1003,10,'2024-11-29 01:49:07',2),(4,1001,1002,17,'2024-11-29 02:08:34',NULL),(5,1002,1001,17,'2024-11-29 02:13:45',4);
/*!40000 ALTER TABLE `lotmovements` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-11-30  4:13:56
