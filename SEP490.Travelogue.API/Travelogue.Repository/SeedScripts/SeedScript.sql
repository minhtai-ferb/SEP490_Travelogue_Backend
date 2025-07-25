-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: travelogue
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP DATABASE IF EXISTS `travelogue2`;
CREATE DATABASE `travelogue2` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `travelogue2`;

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20250724143834_InitDb','8.0.8');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `announcements`
--

DROP TABLE IF EXISTS `announcements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announcements` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `title` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_announcements_tour_guide_id` (`tour_guide_id`),
  KEY `IX_announcements_tour_id` (`tour_id`),
  CONSTRAINT `FK_announcements_tour_guides_tour_guide_id` FOREIGN KEY (`tour_guide_id`) REFERENCES `tour_guides` (`id`),
  CONSTRAINT `FK_announcements_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announcements`
--

LOCK TABLES `announcements` WRITE;
/*!40000 ALTER TABLE `announcements` DISABLE KEYS */;
/*!40000 ALTER TABLE `announcements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `booking_participant`
--

DROP TABLE IF EXISTS `booking_participant`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `booking_participant` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `booking_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `type` int NOT NULL,
  `quantity` int NOT NULL,
  `price_per_participant` decimal(65,30) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_booking_participant_booking_id` (`booking_id`),
  CONSTRAINT `FK_booking_participant_bookings_booking_id` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `booking_participant`
--

LOCK TABLES `booking_participant` WRITE;
/*!40000 ALTER TABLE `booking_participant` DISABLE KEYS */;
/*!40000 ALTER TABLE `booking_participant` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `booking_withdrawals`
--

DROP TABLE IF EXISTS `booking_withdrawals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `booking_withdrawals` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `booking_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `withdrawal_request_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `amount` decimal(18,2) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_booking_withdrawals_booking_id` (`booking_id`),
  KEY `IX_booking_withdrawals_withdrawal_request_id` (`withdrawal_request_id`),
  CONSTRAINT `FK_booking_withdrawals_bookings_booking_id` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_booking_withdrawals_withdrawal_requests_withdrawal_request_id` FOREIGN KEY (`withdrawal_request_id`) REFERENCES `withdrawal_requests` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `booking_withdrawals`
--

LOCK TABLES `booking_withdrawals` WRITE;
/*!40000 ALTER TABLE `booking_withdrawals` DISABLE KEYS */;
/*!40000 ALTER TABLE `booking_withdrawals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bookings`
--

DROP TABLE IF EXISTS `bookings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bookings` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `tour_schedule_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `workshop_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `payment_link_id` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `status` int NOT NULL,
  `booking_type` int NOT NULL,
  `booking_date` datetime(6) NOT NULL,
  `cancelled_at` datetime(6) DEFAULT NULL,
  `is_open_to_join` tinyint(1) NOT NULL,
  `promotion_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `original_price` decimal(10,2) NOT NULL,
  `discount_amount` decimal(10,2) NOT NULL,
  `final_price` decimal(10,2) NOT NULL,
  `trip_plan_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_bookings_promotion_id` (`promotion_id`),
  KEY `IX_bookings_tour_guide_id` (`tour_guide_id`),
  KEY `IX_bookings_tour_id` (`tour_id`),
  KEY `IX_bookings_tour_schedule_id` (`tour_schedule_id`),
  KEY `IX_bookings_trip_plan_id` (`trip_plan_id`),
  KEY `IX_bookings_user_id` (`user_id`),
  KEY `IX_bookings_workshop_id` (`workshop_id`),
  CONSTRAINT `FK_bookings_promotions_promotion_id` FOREIGN KEY (`promotion_id`) REFERENCES `promotions` (`id`),
  CONSTRAINT `FK_bookings_tour_guides_tour_guide_id` FOREIGN KEY (`tour_guide_id`) REFERENCES `tour_guides` (`id`),
  CONSTRAINT `FK_bookings_tour_schedules_tour_schedule_id` FOREIGN KEY (`tour_schedule_id`) REFERENCES `tour_schedules` (`id`),
  CONSTRAINT `FK_bookings_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`),
  CONSTRAINT `FK_bookings_trip_plans_trip_plan_id` FOREIGN KEY (`trip_plan_id`) REFERENCES `trip_plans` (`id`),
  CONSTRAINT `FK_bookings_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_bookings_workshops_workshop_id` FOREIGN KEY (`workshop_id`) REFERENCES `workshops` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bookings`
--

LOCK TABLES `bookings` WRITE;
/*!40000 ALTER TABLE `bookings` DISABLE KEYS */;
/*!40000 ALTER TABLE `bookings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `certifications`
--

DROP TABLE IF EXISTS `certifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `certifications` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `certificate_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_certifications_tour_guide_id` (`tour_guide_id`),
  CONSTRAINT `FK_certifications_tour_guides_tour_guide_id` FOREIGN KEY (`tour_guide_id`) REFERENCES `tour_guides` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `certifications`
--

LOCK TABLES `certifications` WRITE;
/*!40000 ALTER TABLE `certifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `certifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `craft_village_requests`
--

DROP TABLE IF EXISTS `craft_village_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `craft_village_requests` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `address` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `latitude` double NOT NULL,
  `longitude` double NOT NULL,
  `open_time` time(6) DEFAULT NULL,
  `close_time` time(6) DEFAULT NULL,
  `district_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `phone_number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `website` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `owner_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `workshops_available` tinyint(1) NOT NULL,
  `signature_product` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `years_of_history` int DEFAULT NULL,
  `is_recognized_by_unesco` tinyint(1) NOT NULL,
  `status` int NOT NULL,
  `rejection_reason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `reviewed_at` datetime(6) DEFAULT NULL,
  `reviewed_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `craft_village_requests`
--

LOCK TABLES `craft_village_requests` WRITE;
/*!40000 ALTER TABLE `craft_village_requests` DISABLE KEYS */;
/*!40000 ALTER TABLE `craft_village_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `craft_villages`
--

DROP TABLE IF EXISTS `craft_villages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `craft_villages` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `phone_number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `website` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `owner_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `workshops_available` tinyint(1) NOT NULL,
  `signature_product` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `years_of_history` int DEFAULT NULL,
  `is_recognized_by_unesco` tinyint(1) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_craft_villages_location_id` (`location_id`),
  KEY `IX_craft_villages_owner_id` (`owner_id`),
  CONSTRAINT `FK_craft_villages_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_craft_villages_users_owner_id` FOREIGN KEY (`owner_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `craft_villages`
--

LOCK TABLES `craft_villages` WRITE;
/*!40000 ALTER TABLE `craft_villages` DISABLE KEYS */;
INSERT INTO `craft_villages` VALUES ('2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d4e','0276-345-6789','phuocdongmaytre@gmail.com','www.phuocdongmaytre.vn','2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d4e','08ddcac0-a740-4e88-8adb-4b92b6424b39',0,'',2000,0,'2021-11-20 08:30:00.000000','2021-11-20 08:30:00.000000',NULL,'admin','admin','1',0,8),('6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a2b','0276-123-4567','tanbinhgom@gmail.com','www.tanbinhgom.vn','6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a2b','08ddcac0-a740-4e88-8adb-4b92b6424b39',1,'',2000,0,'2023-01-15 10:00:00.000000','2021-11-20 08:30:00.000000',NULL,'admin','admin','1',0,8),('7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e5f','0276-456-7890','muoitrangbang@gmail.com','www.muoitrangbang.vn','7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e5f','08ddcac0-a740-4e88-8adb-4b92b6424b39',1,'',2000,0,'2020-03-05 07:00:00.000000','2021-11-20 08:30:00.000000',NULL,'admin','user2','1',0,8),('8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e6a','0276-567-8901','banhtrangtayninh@gmail.com','www.banhtrangtayninh.vn','8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e6a','08ddcac0-a740-4e88-8adb-4b92b6424b39',1,'',2000,0,'2019-08-12 06:45:00.000000','2021-11-20 08:30:00.000000',NULL,'admin','admin','1',0,8),('9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c3d','0276-234-5678','chamthocam@gmail.com','www.chamthocam.vn','9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c3d','08ddcac0-a740-4e88-8adb-4b92b6424b39',1,'',2000,0,'2022-05-10 09:15:00.000000','2021-11-20 08:30:00.000000',NULL,'admin','user1','1',0,8);
/*!40000 ALTER TABLE `craft_villages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisines`
--

DROP TABLE IF EXISTS `cuisines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisines` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `cuisine_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `phone_number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `website` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `signature_product` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `cooking_method` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_cuisines_location_id` (`location_id`),
  CONSTRAINT `FK_cuisines_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisines`
--

LOCK TABLES `cuisines` WRITE;
/*!40000 ALTER TABLE `cuisines` DISABLE KEYS */;
INSERT INTO `cuisines` VALUES ('a9b2c3d4-e5f6-4a7b-8901-bcde23456789','BÃ² tÆ¡ TÃ¢y Ninh','0276-789-2345','bototayninh@gmail.com','www.bototayninh.vn','a9b2c3d4-e5f6-4a7b-8901-bcde23456789','','','2021-07-20 09:30:00.000000','2025-05-10 14:00:00.000000',NULL,'admin','user1','',1,0),('b0c3d4e5-f6a7-4b8c-9012-cdef34567890','ChÃ¨ truyá»n thá»ng','0276-890-3456','chebuoibanam@gmail.com','www.chebuoibanam.vn','b0c3d4e5-f6a7-4b8c-9012-cdef34567890','','','2020-11-15 07:45:00.000000','2025-04-20 12:15:00.000000',NULL,'admin','admin','',1,0),('c1d4e5f6-a7b8-4c9d-0123-def456789012','Háº£i sáº£n dÃ¢n dÃ£','0276-901-4567','ocdongque@gmail.com','www.ocdongque.vn','c1d4e5f6-a7b8-4c9d-0123-def456789012','','','2023-03-05 10:00:00.000000','2025-06-01 16:45:00.000000',NULL,'admin','user2','',1,0),('d2e5f6a7-b8c9-4d0e-1234-efa567890123','BÃ¡nh xÃ¨o miá»n TÃ¢y','0276-012-5678','banhxeotayninh@gmail.com','www.banhxeotayninh.vn','d2e5f6a7-b8c9-4d0e-1234-efa567890123','','','2019-09-25 06:30:00.000000','2025-03-10 11:00:00.000000',NULL,'admin','admin','',1,0),('f2e1d0c9-b8a7-6655-4433-221100fedcba','BÃ¡nh trÃ¡ng TÃ¢y Ninh','0276-678-1234','banhtrangcoba@gmail.com','www.banhtrangcoba.vn','f2e1d0c9-b8a7-6655-4433-221100fedcba','','','2022-02-10 08:00:00.000000','2025-06-15 15:30:00.000000',NULL,'admin','admin','',1,0);
/*!40000 ALTER TABLE `cuisines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `district_medias`
--

DROP TABLE IF EXISTS `district_medias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `district_medias` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `media_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `file_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `file_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `size_in_bytes` float NOT NULL,
  `district_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_district_medias_district_id` (`district_id`),
  CONSTRAINT `FK_district_medias_districts_district_id` FOREIGN KEY (`district_id`) REFERENCES `districts` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `district_medias`
--

LOCK TABLES `district_medias` WRITE;
/*!40000 ALTER TABLE `district_medias` DISABLE KEYS */;
/*!40000 ALTER TABLE `district_medias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `districts`
--

DROP TABLE IF EXISTS `districts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `districts` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `area` float DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `districts`
--

LOCK TABLES `districts` WRITE;
/*!40000 ALTER TABLE `districts` DISABLE KEYS */;
INSERT INTO `districts` VALUES ('08dd7320-e584-4f18-8906-45c7ee5f34f2','TÃ¢y Ninh','ThÃ nh phá» TÃ¢y Ninh lÃ  trung tÃ¢m hÃ nh chÃ­nh, kinh táº¿ vÃ  vÄn hÃ³a cá»§a tá»nh. ThÃ nh phá» nÃ y cÃ³ nhiá»u di tÃ­ch lá»ch sá»­, vÄn hÃ³a, vÃ  lÃ  nÆ¡i Äáº·t trá»¥ sá» cá»§a nhiá»u cÆ¡ quan quan trá»ng cá»§a tá»nh. ThÃ nh phá» TÃ¢y Ninh cÃ³ 7 phÆ°á»ng vÃ  3 xÃ£.',139.9,'2025-04-04 02:32:08.086183','2025-04-04 02:32:08.086183',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-193f-4c71-8c55-49696e54af47','Báº¿n Cáº§u','Náº±m á» phÃ­a TÃ¢y Nam cá»§a tá»nh, huyá»n Báº¿n Cáº§u giÃ¡p vá»i Campuchia, lÃ  cá»­a ngÃµ giao thÆ°Æ¡ng quan trá»ng qua cá»­a kháº©u Má»c BÃ i. Kinh táº¿ huyá»n phÃ¡t triá»n chá»§ yáº¿u dá»±a vÃ o nÃ´ng nghiá»p vÃ  thÆ°Æ¡ng máº¡i biÃªn giá»i. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  8 xÃ£.',264,'2025-04-04 02:33:35.083950','2025-04-04 02:33:35.083950',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-2d88-4011-8872-50c073c91357','ChÃ¢u ThÃ nh','Náº±m á» phÃ­a TÃ¢y Báº¯c cá»§a tá»nh, huyá»n ChÃ¢u ThÃ nh cÃ³ ná»n kinh táº¿ chá»§ yáº¿u dá»±a vÃ o nÃ´ng nghiá»p, vá»i cÃ¡c cÃ¢y trá»ng chÃ­nh lÃ  lÃºa, cao su, vÃ  mÃ­a. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  14 xÃ£.',580.94,'2025-04-04 02:34:09.142058','2025-04-04 02:34:09.142058',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-5dee-4848-82bf-96526ccf9b64','DÆ°Æ¡ng Minh ChÃ¢u','Náº±m á» phÃ­a ÄÃ´ng cá»§a tá»nh, huyá»n DÆ°Æ¡ng Minh ChÃ¢u ná»i tiáº¿ng vá»i há» Dáº§u Tiáº¿ng, má»t trong nhá»¯ng há» nÆ°á»c nhÃ¢n táº¡o lá»n nháº¥t Viá»t Nam. Kinh táº¿ huyá»n chá»§ yáº¿u dá»±a vÃ o nÃ´ng nghiá»p vÃ  thá»§y sáº£n. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  10 xÃ£.',435.6,'2025-04-04 02:35:30.344026','2025-04-04 02:35:30.344026',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-6dbf-4540-8d29-32fdf3aed359','GÃ² Dáº§u','Náº±m á» phÃ­a Nam cá»§a tá»nh, huyá»n GÃ² Dáº§u cÃ³ ná»n kinh táº¿ phÃ¡t triá»n Äa dáº¡ng, tá»« nÃ´ng nghiá»p, cÃ´ng nghiá»p Äáº¿n thÆ°Æ¡ng máº¡i, Äáº·c biá»t lÃ  khu cÃ´ng nghiá»p PhÆ°á»c ÄÃ´ng - Bá»i Lá»i. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  8 xÃ£.',260,'2025-04-04 02:35:56.878358','2025-04-04 02:35:56.878358',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-7d51-4377-825c-8bbef8aed848','HÃ²a ThÃ nh','Náº±m á» phÃ­a Nam cá»§a tá»nh, HÃ²a ThÃ nh ná»i tiáº¿ng vá»i TÃ²a ThÃ¡nh Cao ÄÃ i, má»t trong nhá»¯ng cÃ´ng trÃ¬nh kiáº¿n trÃºc tÃ´n giÃ¡o Äá»c ÄÃ¡o vÃ  quan trá»ng cá»§a Viá»t Nam. Kinh táº¿ huyá»n phÃ¡t triá»n chá»§ yáº¿u dá»±a vÃ o dá»ch vá»¥ vÃ  nÃ´ng nghiá»p. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  7 xÃ£.',82.92,'2025-04-04 02:36:23.002479','2025-04-04 02:36:23.002479',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-8eb9-4fde-8247-97adc282cd05','TÃ¢n BiÃªn',' LÃ  huyá»n náº±m á» phÃ­a Báº¯c cá»§a tá»nh, giÃ¡p vá»i Campuchia, TÃ¢n BiÃªn cÃ³ Äá»a hÃ¬nh Äá»i nÃºi vÃ  kinh táº¿ chá»§ yáº¿u dá»±a vÃ o nÃ´ng nghiá»p, lÃ¢m nghiá»p vÃ  thÆ°Æ¡ng máº¡i biÃªn giá»i. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  9 xÃ£.',861,'2025-04-04 02:36:52.210760','2025-04-04 02:36:52.210760',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-a2ee-42a9-84fb-964fcadee90e','TÃ¢n ChÃ¢u','Náº±m á» phÃ­a Báº¯c cá»§a tá»nh, huyá»n TÃ¢n ChÃ¢u cÃ³ ná»n kinh táº¿ phÃ¡t triá»n chá»§ yáº¿u dá»±a vÃ o nÃ´ng nghiá»p, vá»i cÃ¡c sáº£n pháº©m chÃ­nh lÃ  cao su, mÃ­a vÃ  cÃ¢y Än trÃ¡i. Huyá»n cÃ³ 1 thá» tráº¥n vÃ  11 xÃ£.',1103.2,'2025-04-04 02:37:26.107277','2025-04-04 02:37:26.107277',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1),('08dd7321-b20f-47ba-8481-964ff9e96b57','Tráº£ng BÃ ng','Náº±m á» phÃ­a ÄÃ´ng Nam cá»§a tá»nh, huyá»n Tráº£ng BÃ ng ná»i tiáº¿ng vá»i lÃ ng nghá» bÃ¡nh trÃ¡ng vÃ  Äáº·c sáº£n bÃ¡nh trÃ¡ng phÆ¡i sÆ°Æ¡ng. Kinh táº¿ huyá»n phÃ¡t triá»n Äa dáº¡ng tá»« nÃ´ng nghiá»p Äáº¿n cÃ´ng nghiá»p vÃ  dá»ch vá»¥. Huyá»n cÃ³ 2 thá» tráº¥n vÃ  8 xÃ£.',340.14,'2025-04-04 02:37:51.489251','2025-04-04 02:37:51.489251',NULL,'1632479d-0297-4080-9e2a-fa8a8dbce6f9','1632479d-0297-4080-9e2a-fa8a8dbce6f9','',1,1);
/*!40000 ALTER TABLE `districts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `favorite_locations`
--

DROP TABLE IF EXISTS `favorite_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `favorite_locations` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_favorite_locations_location_id` (`location_id`),
  KEY `IX_favorite_locations_user_id` (`user_id`),
  CONSTRAINT `FK_favorite_locations_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_favorite_locations_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `favorite_locations`
--

LOCK TABLES `favorite_locations` WRITE;
/*!40000 ALTER TABLE `favorite_locations` DISABLE KEYS */;
/*!40000 ALTER TABLE `favorite_locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historical_locations`
--

DROP TABLE IF EXISTS `historical_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historical_locations` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `heritage_rank` int NOT NULL,
  `established_date` datetime(6) DEFAULT NULL,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `type_historical_location` int DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_historical_locations_location_id` (`location_id`),
  CONSTRAINT `FK_historical_locations_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historical_locations`
--

LOCK TABLES `historical_locations` WRITE;
/*!40000 ALTER TABLE `historical_locations` DISABLE KEYS */;
INSERT INTO `historical_locations` VALUES ('1b2c3d4e-5f6a-4b7c-8901-23456789abcd',3,'1926-01-01 00:00:00.000000','1b2c3d4e-5f6a-4b7c-8901-23456789abcd',1,'2020-01-10 07:00:00.000000','2025-07-01 09:30:00.000000',NULL,'admin','admin',NULL,1,0),('2c3d4e5f-6a7b-4c8d-9012-34567890bcde',2,'1200-01-01 00:00:00.000000','2c3d4e5f-6a7b-4c8d-9012-34567890bcde',2,'2021-03-15 08:15:00.000000','2025-06-20 14:45:00.000000',NULL,'admin','user1',NULL,1,0),('3d4e5f6a-7b8c-4d9e-0123-45678901cdef',3,'1962-01-01 00:00:00.000000','3d4e5f6a-7b8c-4d9e-0123-45678901cdef',3,'2019-11-20 06:30:00.000000','2025-05-15 11:20:00.000000',NULL,'admin','admin',NULL,1,0),('4e5f6a7b-8c9d-4e0f-1234-56789012def0',1,'1800-01-01 00:00:00.000000','4e5f6a7b-8c9d-4e0f-1234-56789012def0',4,'2020-06-05 09:00:00.000000','2025-04-10 13:00:00.000000',NULL,'admin','user2',NULL,1,0),('5f6a7b8c-9d0e-4f1a-2345-67890123ef01',0,'1965-01-01 00:00:00.000000','5f6a7b8c-9d0e-4f1a-2345-67890123ef01',3,'2022-08-12 07:45:00.000000','2025-03-25 10:15:00.000000',NULL,'admin','admin',NULL,1,0);
/*!40000 ALTER TABLE `historical_locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `location_interests`
--

DROP TABLE IF EXISTS `location_interests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `location_interests` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `interest` int NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_location_interests_location_id` (`location_id`),
  CONSTRAINT `FK_location_interests_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `location_interests`
--

LOCK TABLES `location_interests` WRITE;
/*!40000 ALTER TABLE `location_interests` DISABLE KEYS */;
/*!40000 ALTER TABLE `location_interests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `location_medias`
--

DROP TABLE IF EXISTS `location_medias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `location_medias` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `media_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `file_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `file_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `size_in_bytes` float NOT NULL,
  `is_thumbnail` tinyint(1) NOT NULL,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_location_medias_location_id` (`location_id`),
  CONSTRAINT `FK_location_medias_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `location_medias`
--

LOCK TABLES `location_medias` WRITE;
/*!40000 ALTER TABLE `location_medias` DISABLE KEYS */;
INSERT INTO `location_medias` VALUES ('08ddc73f-e5a8-4089-8b8f-47ea37b8f4a1','1','1','1',1,1,'08ddc73f-e5a8-4089-8b8f-47ea37b8f4a0','2025-07-20 13:08:14.615000','2025-07-20 13:08:14.000000',NULL,'','','',1,0),('1a2b3c4d-5e6f-7788-9900-aabbccddeef1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'1a2b3c4d-5e6f-7788-9900-aabbccddeeff','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('1b2c3d4e-5f6a-4b7c-8901-23456789abc1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'1b2c3d4e-5f6a-4b7c-8901-23456789abcd','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d41','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d4e','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('2c3d4e5f-6a7b-4c8d-9012-34567890bcd2','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'2c3d4e5f-6a7b-4c8d-9012-34567890bcde','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('3d4e5f6a-7b8c-4d9e-0123-45678901cde1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'3d4e5f6a-7b8c-4d9e-0123-45678901cdef','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('4e5f6a7b-8c9d-4e0f-1234-56789012def1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'4e5f6a7b-8c9d-4e0f-1234-56789012def0','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('5f6a7b8c-9d0e-4f1a-2345-67890123ef11','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'5f6a7b8c-9d0e-4f1a-2345-67890123ef01','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a21','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a2b','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e51','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e5f','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e61','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e6a','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('98765432-10fe-dcba-9876-543210fedcb1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'98765432-10fe-dcba-9876-543210fedcba','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c31','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c3d','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('a1b2c3d4-e5f6-7890-1234-567890abcde1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'a1b2c3d4-e5f6-7890-1234-567890abcdef','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('a9b2c3d4-e5f6-4a7b-8901-bcde23456781','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'a9b2c3d4-e5f6-4a7b-8901-bcde23456789','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('abcdef98-7654-3210-fedc-ba9876543211','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'abcdef98-7654-3210-fedc-ba9876543210','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('b0c3d4e5-f6a7-4b8c-9012-cdef34567891','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'b0c3d4e5-f6a7-4b8c-9012-cdef34567890','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('c1d4e5f6-a7b8-4c9d-0123-def456789011','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'c1d4e5f6-a7b8-4c9d-0123-def456789012','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('d2e5f6a7-b8c9-4d0e-1234-efa567890121','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'d2e5f6a7-b8c9-4d0e-1234-efa567890123','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0),('f2e1d0c9-b8a7-6655-4433-221100fedcb1','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','1	1','1	1',1,1,'f2e1d0c9-b8a7-6655-4433-221100fedcba','2025-07-21 04:48:05.000000','2025-07-21 04:48:05.000000',NULL,'','','',1,0);
/*!40000 ALTER TABLE `location_medias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `locations`
--

DROP TABLE IF EXISTS `locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `locations` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `address` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `latitude` double NOT NULL,
  `longitude` double NOT NULL,
  `open_time` time(6) DEFAULT NULL,
  `close_time` time(6) DEFAULT NULL,
  `district_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `location_type` int NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_locations_district_id` (`district_id`),
  CONSTRAINT `FK_locations_districts_district_id` FOREIGN KEY (`district_id`) REFERENCES `districts` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `locations`
--

LOCK TABLES `locations` WRITE;
/*!40000 ALTER TABLE `locations` DISABLE KEYS */;
INSERT INTO `locations` VALUES ('08ddc73f-e5a8-4089-8b8f-47ea37b8f4a0','string','string','string','','string',0,180,NULL,NULL,'08dd7320-e584-4f18-8906-45c7ee5f34f2',1,'2025-07-24 20:17:07.729000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('1a2b3c4d-5e6f-7788-9900-aabbccddeeff','Luxe Resort & Spa TÃ¢y Ninh','Khu nghá» dÆ°á»¡ng sang trá»ng vá»i spa Äáº³ng cáº¥p vÃ  há» bÆ¡i vÃ´ cá»±c, mang láº¡i tráº£i nghiá»m thÆ° giÃ£n tá»i Äa.','Luxe Resort & Spa TÃ¢y Ninh lÃ  Äiá»m Äáº¿n lÃ½ tÆ°á»ng cho nhá»¯ng ai tÃ¬m kiáº¿m sá»± thÆ° thÃ¡i vÃ  sang trá»ng. Náº±m áº©n mÃ¬nh giá»¯a thiÃªn nhiÃªn yÃªn bÃ¬nh, khu nghá» dÆ°á»¡ng cÃ³ cÃ¡c biá»t thá»± riÃªng tÆ°, há» bÆ¡i vÃ´ cá»±c nhÃ¬n ra nÃºi BÃ  Äen, vÃ  má»t spa trá» liá»u toÃ n diá»n. NhÃ  hÃ ng phá»¥c vá»¥ cÃ¡c mÃ³n Än organic tÆ°Æ¡i ngon.','','Khu du lá»ch NÃºi BÃ  Äen, XÃ£ Tháº¡nh TÃ¢n, TÃ¢y Ninh',11.36789,106.076543,'00:00:00.000000','23:59:59.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('1b2c3d4e-5f6a-4b7c-8901-23456789abcd','TÃ²a ThÃ¡nh Cao ÄÃ i TÃ¢y Ninh','Trung tÃ¢m tÃ´n giÃ¡o Cao ÄÃ i, di tÃ­ch vÄn hÃ³a vÃ  lá»ch sá»­ ná»i tiáº¿ng.','CÃ´ng trÃ¬nh kiáº¿n trÃºc Äá»c ÄÃ¡o, nÆ¡i diá»n ra cÃ¡c nghi lá» Cao ÄÃ i, thu hÃºt du khÃ¡ch vÃ  tÃ­n Äá».','','PhÆ°á»ng Long Hoa, XÃ£ Long ThÃ nh Báº¯c, Huyá»n HÃ²a ThÃ nh',11.2605,106.0902,'06:00:00.000000','18:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d4e','LÃ ng nghá» mÃ¢y tre Äan PhÆ°á»c ÄÃ´ng','LÃ ng nghá» chuyÃªn sáº£n xuáº¥t sáº£n pháº©m mÃ¢y tre Äan thá»§ cÃ´ng.','Sáº£n xuáº¥t giá», bÃ n gháº¿, Äá» trang trÃ­ tá»« mÃ¢y tre vá»i ká»¹ thuáº­t Äan tinh xáº£o.','','áº¤p PhÆ°á»c Äá»©c B, XÃ£ PhÆ°á»c ÄÃ´ng, Huyá»n GÃ² Dáº§u',11.1652,106.0154,'08:00:00.000000','17:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('2c3d4e5f-6a7b-4c8d-9012-34567890bcde','ThÃ¡p ChÃ³t Máº¡t','ThÃ¡p cá» cá»§a ngÆ°á»i ChÄm, di tÃ­ch lá»ch sá»­ lÃ¢u Äá»i táº¡i TÃ¢y Ninh.','ThÃ¡p ChÄm cá» vá»i kiáº¿n trÃºc Äáº·c trÆ°ng, chá»©ng tÃ­ch vÄn hÃ³a ChÄm Pa, ÄÆ°á»£c báº£o tá»n cáº©n tháº­n.','','áº¤p ChÃ³t Máº¡t, XÃ£ TÃ¢n Phong, Huyá»n TÃ¢n BiÃªn',11.5503,105.9507,'07:30:00.000000','17:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('3d4e5f6a-7b8c-4d9e-0123-45678901cdef','CÄn cá»© Trung Æ°Æ¡ng Cá»¥c miá»n Nam','Di tÃ­ch lá»ch sá»­ khÃ¡ng chiáº¿n, nÆ¡i hoáº¡t Äá»ng cá»§a cÃ¡ch máº¡ng Viá»t Nam.','Khu cÄn cá»© vá»i háº§m trÃº áº©n, nhÃ  lÃ m viá»c, minh chá»©ng cho cuá»c khÃ¡ng chiáº¿n chá»ng Má»¹.','','áº¤p BÃ u ÄÃ´n, XÃ£ TÃ¢n Láº­p, Huyá»n TÃ¢n BiÃªn',11.5801,105.9204,'07:00:00.000000','17:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('4e5f6a7b-8c9d-4e0f-1234-56789012def0','NÃºi BÃ  Äen','NÃºi thiÃªng vÃ  di tÃ­ch lá»ch sá»­, gáº¯n vá»i vÄn hÃ³a tÃ¢m linh TÃ¢y Ninh.','Khu vá»±c chÃ¹a BÃ , Äiá»n thá», vÃ  cÃ¡c di tÃ­ch liÃªn quan, Äiá»m hÃ nh hÆ°Æ¡ng vÃ  du lá»ch ná»i tiáº¿ng.','','PhÆ°á»ng Ninh SÆ¡n, TP. TÃ¢y Ninh',11.3402,106.1405,'06:00:00.000000','20:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('5f6a7b8c-9d0e-4f1a-2345-67890123ef01','Äá»a Äáº¡o Suá»i SÃ¢u','Há» thá»ng Äá»a Äáº¡o lá»ch sá»­ tá»« thá»i khÃ¡ng chiáº¿n chá»ng Má»¹.','Äá»a Äáº¡o ÄÆ°á»£c xÃ¢y dá»±ng kiÃªn cá», phá»¥c vá»¥ chiáº¿n Äáº¥u vÃ  sinh hoáº¡t cá»§a quÃ¢n dÃ¢n trong khÃ¡ng chiáº¿n.','','áº¤p Suá»i SÃ¢u, XÃ£ PhÆ°á»c Vinh, Huyá»n ChÃ¢u ThÃ nh',11.2856,106.0989,'08:00:00.000000','16:30:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a2b','LÃ ng nghá» gá»m TÃ¢n BÃ¬nh','LÃ ng nghá» sáº£n xuáº¥t gá»m truyá»n thá»ng lÃ¢u Äá»i táº¡i TÃ¢y Ninh.','Sáº£n xuáº¥t cÃ¡c sáº£n pháº©m gá»m sá»© thá»§ cÃ´ng nhÆ° chÃ©n, bÃ¡t, bÃ¬nh hoa vá»i ká»¹ thuáº­t cá» truyá»n.','','áº¤p TÃ¢n BÃ¬nh, XÃ£ TÃ¢n BÃ¬nh, TP. TÃ¢y Ninh',11.3087,106.1285,'08:00:00.000000','17:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e5f','LÃ ng nghá» lÃ m muá»i Tráº£ng BÃ ng','LÃ ng nghá» sáº£n xuáº¥t muá»i truyá»n thá»ng vá»i quy trÃ¬nh thá»§ cÃ´ng Äáº·c trÆ°ng.','Sáº£n xuáº¥t muá»i háº¡t vÃ  muá»i tinh tá»« cÃ¡c má» muá»i tá»± nhiÃªn, phá»¥c vá»¥ áº©m thá»±c vÃ  xuáº¥t kháº©u.','','áº¤p Lá»c Trá», XÃ£ Gia Lá»c, Huyá»n Tráº£ng BÃ ng',11.0301,106.1658,'06:00:00.000000','18:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e6a','LÃ ng nghá» bÃ¡nh trÃ¡ng phÆ¡i sÆ°Æ¡ng','LÃ ng nghá» ná»i tiáº¿ng vá»i bÃ¡nh trÃ¡ng phÆ¡i sÆ°Æ¡ng Äáº·c sáº£n TÃ¢y Ninh.','Sáº£n xuáº¥t bÃ¡nh trÃ¡ng phÆ¡i sÆ°Æ¡ng vá»i ká»¹ thuáº­t phÆ¡i ÄÃªm Äá»c ÄÃ¡o, phá»¥c vá»¥ áº©m thá»±c Äá»a phÆ°Æ¡ng.','','áº¤p TrÃ  VÃµ, XÃ£ Tháº¡nh Äá»©c, Huyá»n GÃ² Dáº§u',11.0805,106.0256,'07:00:00.000000','17:30:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('98765432-10fe-dcba-9876-543210fedcba','Homestay BÃ¬nh YÃªn TÃ¢y Ninh','Homestay áº¥m cÃºng, gáº§n gÅ©i vá»i thiÃªn nhiÃªn, thÃ­ch há»£p cho chuyáº¿n Äi nghá» dÆ°á»¡ng yÃªn bÃ¬nh.','Homestay BÃ¬nh YÃªn TÃ¢y Ninh mang Äáº¿n khÃ´ng gian sá»ng má»c máº¡c, gáº§n gÅ©i vá»i cuá»c sá»ng Äá»a phÆ°Æ¡ng. CÃ¡c phÃ²ng ÄÆ°á»£c thiáº¿t káº¿ ÄÆ¡n giáº£n nhÆ°ng áº¥m cÃºng, cÃ³ vÆ°á»n cÃ¢y xanh mÃ¡t. KhÃ¡ch cÃ³ thá» tham gia cÃ¡c hoáº¡t Äá»ng nÃ´ng nghiá»p nhá», thÆ°á»ng thá»©c áº©m thá»±c nhÃ  lÃ m vÃ  khÃ¡m phÃ¡ vÄn hÃ³a Äá»a phÆ°Æ¡ng.','','25 ÄÆ°á»ng HÃ¹ng VÆ°Æ¡ng, Thá» tráº¥n HÃ²a ThÃ nh, TÃ¢y Ninh',11.265432,106.15789,'06:00:00.000000','22:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c3d','LÃ ng nghá» dá»t thá» cáº©m ChÄm','LÃ ng nghá» dá»t thá» cáº©m cá»§a ngÆ°á»i ChÄm vá»i hoa vÄn Äá»c ÄÃ¡o.','Sáº£n xuáº¥t khÄn, Ã¡o, tÃºi xÃ¡ch báº±ng ká»¹ thuáº­t dá»t tay truyá»n thá»ng cá»§a dÃ¢n tá»c ChÄm.','','áº¤p PhÆ°á»c Lá»£i, XÃ£ PhÆ°á»c Vinh, Huyá»n ChÃ¢u ThÃ nh',11.2854,106.0987,'07:30:00.000000','16:30:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('a1b2c3d4-e5f6-7890-1234-567890abcdef','KhÃ¡ch sáº¡n Riverside TÃ¢y Ninh','KhÃ¡ch sáº¡n 4 sao vá»i táº§m nhÃ¬n ra sÃ´ng, há» bÆ¡i vÃ´ cá»±c vÃ  nhÃ  hÃ ng cao cáº¥p.','Tá»a láº¡c bÃªn bá» sÃ´ng VÃ m Cá» ÄÃ´ng, KhÃ¡ch sáº¡n Riverside TÃ¢y Ninh mang Äáº¿n tráº£i nghiá»m nghá» dÆ°á»¡ng sang trá»ng. Vá»i 120 phÃ²ng vÃ  suite ÄÆ°á»£c thiáº¿t káº¿ hiá»n Äáº¡i, táº¥t cáº£ Äá»u cÃ³ ban cÃ´ng riÃªng vÃ  táº§m nhÃ¬n tuyá»t Äáº¹p. KhÃ¡ch sáº¡n cÃ³ 3 nhÃ  hÃ ng phá»¥c vá»¥ áº©m thá»±c Ã - Ãu, quáº§y bar bÃªn há» bÆ¡i, spa trá» liá»u vÃ  phÃ²ng gym hiá»n Äáº¡i. Äá»a Äiá»m lÃ½ tÆ°á»ng cho cáº£ du lá»ch cÃ´ng tÃ¡c vÃ  nghá» dÆ°á»¡ng.','','123 ÄÆ°á»ng 30 ThÃ¡ng 4, PhÆ°á»ng 1, ThÃ nh phá» TÃ¢y Ninh',11.312345,106.109876,NULL,NULL,'08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('a9b2c3d4-e5f6-4a7b-8901-bcde23456789','QuÃ¡n BÃ² TÆ¡ TÃ¢y Ninh','ChuyÃªn cÃ¡c mÃ³n bÃ² tÆ¡ nÆ°á»ng vÃ  láº©u, mÃ³n Än Äáº·c trÆ°ng cá»§a TÃ¢y Ninh.','BÃ² tÆ¡ nÆ°á»ng, láº©u bÃ² tÆ¡, bÃ² tÆ¡ xÃ o lÄn, phá»¥c vá»¥ kÃ¨m rau rá»«ng vÃ  nÆ°á»c cháº¥m Äáº·c biá»t.','','ÄÆ°á»ng 30/4, PhÆ°á»ng 2, TP. TÃ¢y Ninh',11.3102,106.1301,'10:00:00.000000','22:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('abcdef98-7654-3210-fedc-ba9876543210','Green View Hotel Tráº£ng BÃ ng','KhÃ¡ch sáº¡n táº§m trung táº¡i Tráº£ng BÃ ng, thuáº­n tiá»n cho viá»c di chuyá»n vÃ  khÃ¡m phÃ¡ cÃ¡c Äiá»m tham quan.','Green View Hotel Tráº£ng BÃ ng lÃ  lá»±a chá»n lÃ½ tÆ°á»ng cho khÃ¡ch du lá»ch vÃ  cÃ´ng tÃ¡c táº¡i khu vá»±c Tráº£ng BÃ ng. KhÃ¡ch sáº¡n cÃ³ cÃ¡c phÃ²ng sáº¡ch sáº½, tiá»n nghi cÆ¡ báº£n, cÃ¹ng vá»i nhÃ  hÃ ng phá»¥c vá»¥ bá»¯a sÃ¡ng. Vá» trÃ­ thuáº­n lá»£i giÃºp khÃ¡ch dá» dÃ ng di chuyá»n Äáº¿n cÃ¡c khu cÃ´ng nghiá»p vÃ  Äiá»m du lá»ch lÃ¢n cáº­n.','','50 ÄÆ°á»ng Quá»c Lá» 22, Thá» xÃ£ Tráº£ng BÃ ng, TÃ¢y Ninh',11.087654,106.334567,'00:00:00.000000','23:59:59.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('b0c3d4e5-f6a7-4b8c-9012-cdef34567890','ChÃ¨ BÆ°á»i BÃ  NÄm','QuÃ¡n chÃ¨ truyá»n thá»ng ná»i tiáº¿ng vá»i chÃ¨ bÆ°á»i vÃ  cÃ¡c mÃ³n chÃ¨ miá»n TÃ¢y.','ChÃ¨ bÆ°á»i, chÃ¨ thá»t ná»t, chÃ¨ Äáº­u tráº¯ng, sá»­ dá»¥ng nguyÃªn liá»u tÆ°Æ¡i ngon tá»« Äá»a phÆ°Æ¡ng.','','áº¤p Long Thá»i, XÃ£ Long ThÃ nh Trung, Huyá»n HÃ²a ThÃ nh',11.2654,106.0503,'09:00:00.000000','20:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('c1d4e5f6-a7b8-4c9d-0123-def456789012','QuÃ¡n á»c Äá»ng QuÃª','QuÃ¡n bÃ¬nh dÃ¢n chuyÃªn cÃ¡c mÃ³n á»c vÃ  háº£i sáº£n cháº¿ biáº¿n kiá»u TÃ¢y Ninh.','á»c bÆ°Æ¡u nhá»i thá»t, á»c mÃ³ng tay xÃ o me, á»c hÆ°Æ¡ng nÆ°á»ng muá»i á»t, phá»¥c vá»¥ kÃ¨m rau rÄm.','','áº¤p Gia HÃ²a, XÃ£ Gia Lá»c, Huyá»n Tráº£ng BÃ ng',11.0305,106.1652,'11:00:00.000000','23:00:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('d2e5f6a7-b8c9-4d0e-1234-efa567890123','QuÃ¡n BÃ¡nh XÃ¨o Miá»n TÃ¢y','QuÃ¡n bÃ¡nh xÃ¨o truyá»n thá»ng vá»i nhÃ¢n tÃ´m, thá»t vÃ  rau sá»ng Äá»a phÆ°Æ¡ng.','BÃ¡nh xÃ¨o giÃ²n rá»¥m, nhÃ¢n tÃ´m, thá»t heo, giÃ¡ Äá», phá»¥c vá»¥ kÃ¨m nÆ°á»c máº¯m chua ngá»t Äáº·c trÆ°ng.','','áº¤p PhÆ°á»c Äá»©c A, XÃ£ PhÆ°á»c ÄÃ´ng, Huyá»n GÃ² Dáº§u',11.1658,106.0157,'08:00:00.000000','20:30:00.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0),('f2e1d0c9-b8a7-6655-4433-221100fedcba','KhÃ¡ch sáº¡n Golden Palace TÃ¢y Ninh','KhÃ¡ch sáº¡n hiá»n Äáº¡i gáº§n trung tÃ¢m thÃ nh phá», tiá»n nghi Äáº§y Äá»§ vÃ  dá»ch vá»¥ thÃ¢n thiá»n.','KhÃ¡ch sáº¡n Golden Palace TÃ¢y Ninh mang Äáº¿n khÃ´ng gian nghá» dÆ°á»¡ng thoáº£i mÃ¡i vá»i cÃ¡c phÃ²ng nghá» sang trá»ng, Äáº§y Äá»§ tiá»n nghi. Náº±m gáº§n cÃ¡c Äiá»m tham quan chÃ­nh cá»§a thÃ nh phá», khÃ¡ch sáº¡n cung cáº¥p nhÃ  hÃ ng phá»¥c vá»¥ áº©m thá»±c Äá»a phÆ°Æ¡ng vÃ  quá»c táº¿, phÃ²ng há»p vÃ  dá»ch vá»¥ ÄÆ°a ÄÃ³n. LÃ½ tÆ°á»ng cho cáº£ khÃ¡ch du lá»ch vÃ  cÃ´ng tÃ¡c.','','88 ÄÆ°á»ng LÃª Duáº©n, PhÆ°á»ng 3, ThÃ nh phá» TÃ¢y Ninh',11.305,106.102,'00:00:00.000000','23:59:59.000000','08dd7320-e584-4f18-8906-45c7ee5f34f2',2,'2025-07-24 20:17:07.000000','2025-07-24 20:17:07.000000',NULL,'','','',1,0);
/*!40000 ALTER TABLE `locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `messages`
--

DROP TABLE IF EXISTS `messages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `messages` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `receiver_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `sender_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `content` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `sent_at` datetime(6) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_messages_receiver_id` (`receiver_id`),
  KEY `IX_messages_sender_id` (`sender_id`),
  CONSTRAINT `FK_messages_users_receiver_id` FOREIGN KEY (`receiver_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_messages_users_sender_id` FOREIGN KEY (`sender_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messages`
--

LOCK TABLES `messages` WRITE;
/*!40000 ALTER TABLE `messages` DISABLE KEYS */;
/*!40000 ALTER TABLE `messages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `news`
--

DROP TABLE IF EXISTS `news`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `news` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `news_category` int DEFAULT NULL,
  `is_highlighted` tinyint(1) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_news_location_id` (`location_id`),
  CONSTRAINT `FK_news_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `news`
--

LOCK TABLES `news` WRITE;
/*!40000 ALTER TABLE `news` DISABLE KEYS */;
INSERT INTO `news` VALUES ('a9b2c3d4-e5f6-4a7b-8901-bcde23456781','Khai máº¡c Lá» há»i ChÃ¹a BÃ  TÃ¢y Ninh 2025','Lá» há»i ChÃ¹a BÃ  diá»n ra sÃ´i ná»i, thu hÃºt hÃ ng ngÃ n du khÃ¡ch.','Lá» há»i diá»n ra tá»« ngÃ y 20/7/2025 vá»i nhiá»u hoáº¡t Äá»ng vÄn hÃ³a, tÃ¢m linh Äáº·c sáº¯c táº¡i ChÃ¹a BÃ  ThiÃªn Háº­u.',NULL,1,1,'2025-07-20 08:00:00.000000','2025-07-20 10:00:00.000000',NULL,'admin1','admin1','',1,0),('b0c3d4e5-f6a7-4b8c-9012-cdef34567891','TÃ¢y Ninh triá»n khai dá»± Ã¡n ÄÆ°á»ng cao tá»c má»i','Dá»± Ã¡n cao tá»c TÃ¢y Ninh - TP.HCM dá»± kiáº¿n hoÃ n thÃ nh vÃ o 2027.','Tuyáº¿n ÄÆ°á»ng dÃ i 60km, tá»ng vá»n Äáº§u tÆ° 15.000 tá»· Äá»ng, gÃ³p pháº§n thÃºc Äáº©y kinh táº¿ vÃ¹ng.',NULL,2,0,'2025-07-19 09:30:00.000000','2025-07-20 15:00:00.000000',NULL,'editor1','editor2','',1,0),('c1d4e5f6-a7b8-4c9d-0123-def456789011','NÃ´ng dÃ¢n TÃ¢y Ninh trÃºng mÃ¹a mÃ­t ThÃ¡i','Vá»¥ mÃ­t ThÃ¡i nÄm nay Äáº¡t nÄng suáº¥t cao, giÃ¡ bÃ¡n á»n Äá»nh.','NÃ´ng dÃ¢n táº¡i huyá»n TÃ¢n BiÃªn thu hoáº¡ch hÆ¡n 10.000 táº¥n mÃ­t, giÃ¡ trung bÃ¬nh 25.000 VNÄ/kg.',NULL,1,1,'2025-07-18 07:00:00.000000','2025-07-19 12:00:00.000000',NULL,'admin2','admin2','',1,0),('d2e5f6a7-b8c9-4d0e-1234-efa567890121','TÃ¢y Ninh tá» chá»©c há»i thi áº©m thá»±c Äáº·c sáº£n','Há»i thi giá»i thiá»u cÃ¡c mÃ³n Än Äáº·c trÆ°ng nhÆ° bÃ¡nh trÃ¡ng phÆ¡i sÆ°Æ¡ng.','Sá»± kiá»n thu hÃºt 50 Äá»i thi, diá»n ra táº¡i quáº£ng trÆ°á»ng trung tÃ¢m TP. TÃ¢y Ninh vÃ o ngÃ y 22/7/2025.',NULL,2,0,'2025-07-21 10:00:00.000000','2025-07-21 14:00:00.000000',NULL,'editor3','editor3','',1,0),('f8a1b2c3-d4e5-4f6a-7890-abcdef123451','Kiá»m tra an toÃ n thá»±c pháº©m táº¡i chá»£ TÃ¢y Ninh','ÄoÃ n kiá»m tra phÃ¡t hiá»n má»t sá» vi pháº¡m vá» vá» sinh an toÃ n thá»±c pháº©m.','Äá»i kiá»m tra liÃªn ngÃ nh ÄÃ£ xá»­ pháº¡t 5 cÆ¡ sá» vi pháº¡m vÃ  yÃªu cáº§u kháº¯c phá»¥c trong 7 ngÃ y.',NULL,1,0,'2025-07-20 11:00:00.000000','2025-07-21 09:00:00.000000',NULL,'admin3','admin3','',1,0);
/*!40000 ALTER TABLE `news` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `news_medias`
--

DROP TABLE IF EXISTS `news_medias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `news_medias` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `media_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `file_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `file_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `size_in_bytes` float NOT NULL,
  `is_thumbnail` tinyint(1) NOT NULL,
  `news_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_news_medias_news_id` (`news_id`),
  CONSTRAINT `FK_news_medias_news_news_id` FOREIGN KEY (`news_id`) REFERENCES `news` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `news_medias`
--

LOCK TABLES `news_medias` WRITE;
/*!40000 ALTER TABLE `news_medias` DISABLE KEYS */;
/*!40000 ALTER TABLE `news_medias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `password_reset_tokens`
--

DROP TABLE IF EXISTS `password_reset_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `password_reset_tokens` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `token` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `expiry_time` datetime(6) NOT NULL,
  `is_used` tinyint(1) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `password_reset_tokens`
--

LOCK TABLES `password_reset_tokens` WRITE;
/*!40000 ALTER TABLE `password_reset_tokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `password_reset_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `promotion_applicable`
--

DROP TABLE IF EXISTS `promotion_applicable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `promotion_applicable` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `promotion_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `workshop_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `service_type` int NOT NULL,
  `guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_promotion_applicable_guide_id` (`guide_id`),
  KEY `IX_promotion_applicable_promotion_id` (`promotion_id`),
  KEY `IX_promotion_applicable_tour_id` (`tour_id`),
  KEY `IX_promotion_applicable_workshop_id` (`workshop_id`),
  CONSTRAINT `FK_promotion_applicable_promotions_promotion_id` FOREIGN KEY (`promotion_id`) REFERENCES `promotions` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_promotion_applicable_tour_guides_guide_id` FOREIGN KEY (`guide_id`) REFERENCES `tour_guides` (`id`),
  CONSTRAINT `FK_promotion_applicable_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`),
  CONSTRAINT `FK_promotion_applicable_workshops_workshop_id` FOREIGN KEY (`workshop_id`) REFERENCES `workshops` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `promotion_applicable`
--

LOCK TABLES `promotion_applicable` WRITE;
/*!40000 ALTER TABLE `promotion_applicable` DISABLE KEYS */;
/*!40000 ALTER TABLE `promotion_applicable` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `promotions`
--

DROP TABLE IF EXISTS `promotions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `promotions` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `promotion_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `discount_type` int NOT NULL,
  `discount_value` decimal(10,2) NOT NULL,
  `start_date` datetime(6) NOT NULL,
  `end_date` datetime(6) NOT NULL,
  `applicable_type` int NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_promotions_tour_id` (`tour_id`),
  KEY `IX_promotions_user_id` (`user_id`),
  CONSTRAINT `FK_promotions_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_promotions_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `promotions`
--

LOCK TABLES `promotions` WRITE;
/*!40000 ALTER TABLE `promotions` DISABLE KEYS */;
/*!40000 ALTER TABLE `promotions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refund_requests`
--

DROP TABLE IF EXISTS `refund_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refund_requests` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `booking_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `request_date` datetime(6) NOT NULL,
  `reason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `approved_at` datetime(6) DEFAULT NULL,
  `rejection_at` datetime(6) DEFAULT NULL,
  `rejection_reason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `refund_amount` decimal(18,2) DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_refund_requests_booking_id` (`booking_id`),
  KEY `IX_refund_requests_user_id` (`user_id`),
  CONSTRAINT `FK_refund_requests_bookings_booking_id` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_refund_requests_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refund_requests`
--

LOCK TABLES `refund_requests` WRITE;
/*!40000 ALTER TABLE `refund_requests` DISABLE KEYS */;
/*!40000 ALTER TABLE `refund_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reports`
--

DROP TABLE IF EXISTS `reports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reports` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `review_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `reason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `reported_at` datetime(6) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_reports_review_id` (`review_id`),
  KEY `IX_reports_user_id` (`user_id`),
  CONSTRAINT `FK_reports_reviews_review_id` FOREIGN KEY (`review_id`) REFERENCES `reviews` (`id`),
  CONSTRAINT `FK_reports_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reports`
--

LOCK TABLES `reports` WRITE;
/*!40000 ALTER TABLE `reports` DISABLE KEYS */;
/*!40000 ALTER TABLE `reports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reviews`
--

DROP TABLE IF EXISTS `reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reviews` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `booking_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `workshop_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `comment` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `rating` int NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_reviews_booking_id` (`booking_id`),
  KEY `IX_reviews_tour_guide_id` (`tour_guide_id`),
  KEY `IX_reviews_tour_id` (`tour_id`),
  KEY `IX_reviews_user_id` (`user_id`),
  KEY `IX_reviews_workshop_id` (`workshop_id`),
  CONSTRAINT `FK_reviews_bookings_booking_id` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_reviews_tour_guides_tour_guide_id` FOREIGN KEY (`tour_guide_id`) REFERENCES `tour_guides` (`id`),
  CONSTRAINT `FK_reviews_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`),
  CONSTRAINT `FK_reviews_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_reviews_workshops_workshop_id` FOREIGN KEY (`workshop_id`) REFERENCES `workshops` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reviews`
--

LOCK TABLES `reviews` WRITE;
/*!40000 ALTER TABLE `reviews` DISABLE KEYS */;
/*!40000 ALTER TABLE `reviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `normalized_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES ('08ddcabf-ecfe-46fd-85ed-fa3d45413017','Admin','ADMIN',NULL,'2025-07-24 14:39:41.978178','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0),('08ddcabf-ed0d-4cb6-8727-0faa305f54d5','User','USER',NULL,'2025-07-24 14:39:41.983139','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0),('08ddcabf-ed0d-4da3-8eb5-ad7cf1e8bfa5','Moderator','MODERATOR',NULL,'2025-07-24 14:39:41.983144','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0),('08ddcabf-ed0d-4dbb-8d47-4dbbe688c8a0','TourGuide','TOURGUIDE',NULL,'2025-07-24 14:39:41.983144','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0),('08ddcabf-ed0d-4ddf-8f1e-f23cf008b6d0','CraftVillageOwner','CRAFTVILLAGEOWNER',NULL,'2025-07-24 14:39:41.983144','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_settings`
--

DROP TABLE IF EXISTS `system_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_settings` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `value` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_settings`
--

LOCK TABLES `system_settings` WRITE;
/*!40000 ALTER TABLE `system_settings` DISABLE KEYS */;
/*!40000 ALTER TABLE `system_settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_guide_mapping`
--

DROP TABLE IF EXISTS `tour_guide_mapping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_guide_mapping` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_schedule_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_guide_mapping_tour_guide_id` (`tour_guide_id`),
  KEY `IX_tour_guide_mapping_tour_schedule_id` (`tour_schedule_id`),
  CONSTRAINT `FK_tour_guide_mapping_tour_guides_tour_guide_id` FOREIGN KEY (`tour_guide_id`) REFERENCES `tour_guides` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_tour_guide_mapping_tour_schedules_tour_schedule_id` FOREIGN KEY (`tour_schedule_id`) REFERENCES `tour_schedules` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_guide_mapping`
--

LOCK TABLES `tour_guide_mapping` WRITE;
/*!40000 ALTER TABLE `tour_guide_mapping` DISABLE KEYS */;
/*!40000 ALTER TABLE `tour_guide_mapping` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_guide_request_certifications`
--

DROP TABLE IF EXISTS `tour_guide_request_certifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_guide_request_certifications` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_guide_request_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `certificate_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_guide_request_certifications_tour_guide_request_id` (`tour_guide_request_id`),
  CONSTRAINT `FK_tour_guide_request_certifications_tour_guide_requests_tour_g~` FOREIGN KEY (`tour_guide_request_id`) REFERENCES `tour_guide_requests` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_guide_request_certifications`
--

LOCK TABLES `tour_guide_request_certifications` WRITE;
/*!40000 ALTER TABLE `tour_guide_request_certifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `tour_guide_request_certifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_guide_requests`
--

DROP TABLE IF EXISTS `tour_guide_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_guide_requests` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `introduction` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `price` decimal(65,30) NOT NULL,
  `status` int NOT NULL,
  `rejection_reason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `reviewed_at` datetime(6) DEFAULT NULL,
  `reviewed_by` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_guide_requests_user_id` (`user_id`),
  CONSTRAINT `FK_tour_guide_requests_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_guide_requests`
--

LOCK TABLES `tour_guide_requests` WRITE;
/*!40000 ALTER TABLE `tour_guide_requests` DISABLE KEYS */;
/*!40000 ALTER TABLE `tour_guide_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_guide_schedules`
--

DROP TABLE IF EXISTS `tour_guide_schedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_guide_schedules` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_guide_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `booking_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `note` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `date` datetime(6) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_guide_schedules_booking_id` (`booking_id`),
  KEY `IX_tour_guide_schedules_tour_guide_id` (`tour_guide_id`),
  KEY `IX_tour_guide_schedules_tour_id` (`tour_id`),
  CONSTRAINT `FK_tour_guide_schedules_bookings_booking_id` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`),
  CONSTRAINT `FK_tour_guide_schedules_tour_guides_tour_guide_id` FOREIGN KEY (`tour_guide_id`) REFERENCES `tour_guides` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_tour_guide_schedules_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_guide_schedules`
--

LOCK TABLES `tour_guide_schedules` WRITE;
/*!40000 ALTER TABLE `tour_guide_schedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `tour_guide_schedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_guides`
--

DROP TABLE IF EXISTS `tour_guides`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_guides` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `rating` int NOT NULL,
  `price` decimal(65,30) NOT NULL,
  `introduction` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `language_codes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `tag_codes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_guides_user_id` (`user_id`),
  CONSTRAINT `FK_tour_guides_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_guides`
--

LOCK TABLES `tour_guides` WRITE;
/*!40000 ALTER TABLE `tour_guides` DISABLE KEYS */;
INSERT INTO `tour_guides` VALUES ('08ddc745-5c3f-46af-8e04-8743f56b29f5',0,0.000000000000000000000000000000,'','08ddcac0-a740-4e88-8adb-4b92b6424b39','1','1','2025-07-24 20:36:13.081000','2025-07-24 20:36:13.000000',NULL,'','','',1,1);
/*!40000 ALTER TABLE `tour_guides` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_interests`
--

DROP TABLE IF EXISTS `tour_interests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_interests` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `interest` int NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_interests_tour_id` (`tour_id`),
  CONSTRAINT `FK_tour_interests_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_interests`
--

LOCK TABLES `tour_interests` WRITE;
/*!40000 ALTER TABLE `tour_interests` DISABLE KEYS */;
/*!40000 ALTER TABLE `tour_interests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_medias`
--

DROP TABLE IF EXISTS `tour_medias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_medias` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `media_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `file_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `file_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `size_in_bytes` float NOT NULL,
  `is_thumbnail` tinyint(1) NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_medias_tour_id` (`tour_id`),
  CONSTRAINT `FK_tour_medias_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_medias`
--

LOCK TABLES `tour_medias` WRITE;
/*!40000 ALTER TABLE `tour_medias` DISABLE KEYS */;
/*!40000 ALTER TABLE `tour_medias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_plan_locations`
--

DROP TABLE IF EXISTS `tour_plan_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_plan_locations` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `day_order` int NOT NULL,
  `start_time` time(6) NOT NULL,
  `end_time` time(6) NOT NULL,
  `notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `travel_time_from_prev` float NOT NULL,
  `distance_from_prev` float NOT NULL,
  `estimated_start_time` float NOT NULL,
  `estimated_end_time` float NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_plan_locations_location_id` (`location_id`),
  KEY `IX_tour_plan_locations_tour_id` (`tour_id`),
  CONSTRAINT `FK_tour_plan_locations_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_tour_plan_locations_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_plan_locations`
--

LOCK TABLES `tour_plan_locations` WRITE;
/*!40000 ALTER TABLE `tour_plan_locations` DISABLE KEYS */;
INSERT INTO `tour_plan_locations` VALUES ('08ddc740-7062-400c-83f3-5e9dcfd1baa6','08ddc73e-e97f-4d16-845d-7b867ac5a299','08ddc73f-e5a8-4089-8b8f-47ea37b8f4a0',1,'08:00:00.000000','09:00:00.000000','string',0,0,0,0,'2025-07-20 03:49:33.361730','2025-07-23 09:35:02.357187',NULL,'','','',1,1),('08ddc9cc-3313-41a6-8172-0cdf0290ba51','08ddc73e-e97f-4d16-845d-7b867ac5a299','1a2b3c4d-5e6f-7788-9900-aabbccddeeff',1,'08:00:00.000000','09:00:00.000000','Day 1 - Stop 1',0,0,0,0,'2025-07-23 09:35:02.341751','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-43ce-8152-ffbcfc132343','08ddc73e-e97f-4d16-845d-7b867ac5a299','1b2c3d4e-5f6a-4b7c-8901-23456789abcd',1,'09:30:00.000000','10:30:00.000000','Day 1 - Stop 2',30,5,0,0,'2025-07-23 09:35:02.342455','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-44c6-861f-8b9d47fafcd2','08ddc73e-e97f-4d16-845d-7b867ac5a299','2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d4e',1,'11:00:00.000000','12:00:00.000000','Day 1 - Stop 3',30,7,0,0,'2025-07-23 09:35:02.342461','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-4523-8382-a818a79a8a33','08ddc73e-e97f-4d16-845d-7b867ac5a299','2c3d4e5f-6a7b-4c8d-9012-34567890bcde',1,'13:00:00.000000','14:00:00.000000','Day 1 - Stop 4',60,10,0,0,'2025-07-23 09:35:02.342461','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-454d-8775-cb27dc2c6bb9','08ddc73e-e97f-4d16-845d-7b867ac5a299','3d4e5f6a-7b8c-4d9e-0123-45678901cdef',1,'14:30:00.000000','15:30:00.000000','Day 1 - Stop 5',30,3,0,0,'2025-07-23 09:35:02.342462','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-4574-8b0b-ccef9e90beb2','08ddc73e-e97f-4d16-845d-7b867ac5a299','4e5f6a7b-8c9d-4e0f-1234-56789012def0',1,'16:00:00.000000','17:00:00.000000','Day 1 - Stop 6',30,6,0,0,'2025-07-23 09:35:02.342463','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-459b-830d-5d6e82a109a1','08ddc73e-e97f-4d16-845d-7b867ac5a299','5f6a7b8c-9d0e-4f1a-2345-67890123ef01',2,'08:00:00.000000','09:00:00.000000','Day 2 - Stop 1',0,0,0,0,'2025-07-23 09:35:02.342463','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-45c6-87af-9179461c7df0','08ddc73e-e97f-4d16-845d-7b867ac5a299','6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a2b',2,'09:30:00.000000','10:30:00.000000','Day 2 - Stop 2',30,4,0,0,'2025-07-23 09:35:02.342465','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-45fa-8b33-aa1aa938b487','08ddc73e-e97f-4d16-845d-7b867ac5a299','7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e5f',2,'11:00:00.000000','12:00:00.000000','Day 2 - Stop 3',30,8,0,0,'2025-07-23 09:35:02.342465','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-4625-80dd-69a23c646098','08ddc73e-e97f-4d16-845d-7b867ac5a299','8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e6a',2,'13:00:00.000000','14:00:00.000000','Day 2 - Stop 4',60,12,0,0,'2025-07-23 09:35:02.342468','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-465e-870c-8aa655ea7fc8','08ddc73e-e97f-4d16-845d-7b867ac5a299','98765432-10fe-dcba-9876-543210fedcba',2,'14:30:00.000000','15:30:00.000000','Day 2 - Stop 5',30,6,0,0,'2025-07-23 09:35:02.342469','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-4699-821e-ef08790cf7c7','08ddc73e-e97f-4d16-845d-7b867ac5a299','9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c3d',2,'16:00:00.000000','17:00:00.000000','Day 2 - Stop 6',30,5,0,0,'2025-07-23 09:35:02.342469','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-46d3-8de2-d99500f1526b','08ddc73e-e97f-4d16-845d-7b867ac5a299','a1b2c3d4-e5f6-7890-1234-567890abcdef',3,'08:00:00.000000','09:00:00.000000','Day 3 - Stop 1',0,0,0,0,'2025-07-23 09:35:02.342470','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-4723-8077-32bd48a8facb','08ddc73e-e97f-4d16-845d-7b867ac5a299','a9b2c3d4-e5f6-4a7b-8901-bcde23456789',3,'09:30:00.000000','10:30:00.000000','Day 3 - Stop 2',30,4,0,0,'2025-07-23 09:35:02.342470','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-4752-8f5c-599272020fa8','08ddc73e-e97f-4d16-845d-7b867ac5a299','abcdef98-7654-3210-fedc-ba9876543210',3,'11:00:00.000000','12:00:00.000000','Day 3 - Stop 3',30,9,0,0,'2025-07-23 09:35:02.342470','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-477b-83ae-abe20850c13a','08ddc73e-e97f-4d16-845d-7b867ac5a299','b0c3d4e5-f6a7-4b8c-9012-cdef34567890',3,'13:00:00.000000','14:00:00.000000','Day 3 - Stop 4',60,15,0,0,'2025-07-23 09:35:02.342471','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-47a4-8688-cdec111c0f10','08ddc73e-e97f-4d16-845d-7b867ac5a299','c1d4e5f6-a7b8-4c9d-0123-def456789012',3,'14:30:00.000000','15:30:00.000000','Day 3 - Stop 5',30,4,0,0,'2025-07-23 09:35:02.342472','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc9cc-3316-47cc-8334-51a6d03e1f13','08ddc73e-e97f-4d16-845d-7b867ac5a299','d2e5f6a7-b8c9-4d0e-1234-efa567890123',3,'16:00:00.000000','17:00:00.000000','Day 3 - Stop 6',30,7,0,0,'2025-07-23 09:35:02.342472','0001-01-01 00:00:00.000000',NULL,'','','',1,0);
/*!40000 ALTER TABLE `tour_plan_locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tour_schedules`
--

DROP TABLE IF EXISTS `tour_schedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tour_schedules` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `tour_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `departure_date` datetime(6) NOT NULL,
  `max_participant` int NOT NULL,
  `current_booked` int NOT NULL,
  `total_days` int NOT NULL,
  `adult_price` decimal(65,30) NOT NULL,
  `children_price` decimal(65,30) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_tour_schedules_tour_id` (`tour_id`),
  CONSTRAINT `FK_tour_schedules_tours_tour_id` FOREIGN KEY (`tour_id`) REFERENCES `tours` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tour_schedules`
--

LOCK TABLES `tour_schedules` WRITE;
/*!40000 ALTER TABLE `tour_schedules` DISABLE KEYS */;
INSERT INTO `tour_schedules` VALUES ('08ddc740-6933-4ad6-89e7-1186a1f6abde','08ddc73e-e97f-4d16-845d-7b867ac5a299','2025-07-20 03:49:03.638000',2,0,1,10.000000000000000000000000000000,10.000000000000000000000000000000,'2025-07-20 03:49:21.069008','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc740-6933-4ad6-89e7-1186a1f6abdf','08ddc73e-e97f-4d16-845d-7b867ac5a299','2025-07-25 03:49:03.638000',2,0,1,10.000000000000000000000000000000,10.000000000000000000000000000000,'2025-07-20 03:49:21.069008','0001-01-01 00:00:00.000000',NULL,'','','',1,0);
/*!40000 ALTER TABLE `tour_schedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tours`
--

DROP TABLE IF EXISTS `tours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tours` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `total_days` int NOT NULL,
  `status` int NOT NULL,
  `tour_type` int DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tours`
--

LOCK TABLES `tours` WRITE;
/*!40000 ALTER TABLE `tours` DISABLE KEYS */;
INSERT INTO `tours` VALUES ('08ddc73e-e97f-4d16-845d-7b867ac5a291','string','string','string',1,0,1,'2025-07-20 03:38:37.557559','0001-01-01 00:00:00.000000',NULL,'','','',1,0),('08ddc73e-e97f-4d16-845d-7b867ac5a299','string','string','string',3,0,1,'2025-07-20 03:38:37.557559','2025-07-23 09:29:54.275471',NULL,'','','',1,0);
/*!40000 ALTER TABLE `tours` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transactions`
--

DROP TABLE IF EXISTS `transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `transactions` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `booking_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `wallet_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `transaction_date` datetime(6) NOT NULL,
  `status` int NOT NULL,
  `type` int NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_transactions_booking_id` (`booking_id`),
  KEY `IX_transactions_wallet_id` (`wallet_id`),
  CONSTRAINT `FK_transactions_bookings_booking_id` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`id`),
  CONSTRAINT `FK_transactions_wallets_wallet_id` FOREIGN KEY (`wallet_id`) REFERENCES `wallets` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transactions`
--

LOCK TABLES `transactions` WRITE;
/*!40000 ALTER TABLE `transactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `transactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trip_plan_locations`
--

DROP TABLE IF EXISTS `trip_plan_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trip_plan_locations` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `trip_plan_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `location_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `start_time` datetime(6) DEFAULT NULL,
  `end_time` datetime(6) DEFAULT NULL,
  `notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `order` int NOT NULL,
  `travel_time_from_prev` float DEFAULT NULL,
  `distance_from_prev` float DEFAULT NULL,
  `estimated_start_time` float DEFAULT NULL,
  `estimated_end_time` float DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_trip_plan_locations_location_id` (`location_id`),
  KEY `IX_trip_plan_locations_trip_plan_id` (`trip_plan_id`),
  CONSTRAINT `FK_trip_plan_locations_locations_location_id` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_trip_plan_locations_trip_plans_trip_plan_id` FOREIGN KEY (`trip_plan_id`) REFERENCES `trip_plans` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trip_plan_locations`
--

LOCK TABLES `trip_plan_locations` WRITE;
/*!40000 ALTER TABLE `trip_plan_locations` DISABLE KEYS */;
INSERT INTO `trip_plan_locations` VALUES ('08ddc9e4-6359-4c06-8c72-14976661bf93','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','1a2b3c4d-5e6f-7788-9900-aabbccddeeff','2025-07-23 08:00:00.000000','2025-07-23 10:00:00.000000','',1,0,0,0,0,'2025-07-24 20:42:46.865000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-47d2-887b-fdb1d068f80f','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','1b2c3d4e-5f6a-4b7c-8901-23456789abcd','2025-07-23 11:00:00.000000','2025-07-23 13:00:00.000000','',2,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4a23-8958-cee80b5d6881','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','2a8d4e3b-6c9f-4b7a-c1d2-5e0f1b2c3d4e','2025-07-23 14:00:00.000000','2025-07-23 16:00:00.000000','',3,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4a5a-8776-00e28fbd5f84','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','2c3d4e5f-6a7b-4c8d-9012-34567890bcde','2025-07-23 17:00:00.000000','2025-07-23 19:00:00.000000','',4,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4ac5-8bd2-a69cbff5c246','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','3d4e5f6a-7b8c-4d9e-0123-45678901cdef','2025-07-23 20:00:00.000000','2025-07-23 22:00:00.000000','',5,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4aef-80a2-7a4996f5342a','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','4e5f6a7b-8c9d-4e0f-1234-56789012def0','2025-07-24 08:00:00.000000','2025-07-24 10:00:00.000000','',6,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4b16-83a7-db1937946b7a','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','5f6a7b8c-9d0e-4f1a-2345-67890123ef01','2025-07-24 11:00:00.000000','2025-07-24 13:00:00.000000','',7,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4b42-8fae-fa76e31c7e24','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','6b7a2e1f-8c9d-4b5e-a3f2-7c8e9d0f1a2b','2025-07-24 14:00:00.000000','2025-07-24 16:00:00.000000','',8,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4b70-8c33-ddb8c5c114e0','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','7c9e5f4a-3b8d-4c6a-d2e3-6f1a2c3b4e5f','2025-07-24 17:00:00.000000','2025-07-24 19:00:00.000000','',9,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4b95-827a-0cfa4a4a9ff6','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','8d0f6a5b-4c9e-5b7f-e3d4-7a2b3c4d5e6a','2025-07-24 20:00:00.000000','2025-07-24 22:00:00.000000','',10,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4bbb-87d7-931fe406a624','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','98765432-10fe-dcba-9876-543210fedcba','2025-07-24 23:00:00.000000','2025-07-25 01:00:00.000000','',11,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4beb-829c-c14a9d4fbcd6','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','9f4b3c2a-5d7e-4a6f-b8c1-3e9f0a1b2c3d','2025-07-25 02:00:00.000000','2025-07-25 04:00:00.000000','',12,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4c19-8a88-fdef34d2636d','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','a1b2c3d4-e5f6-7890-1234-567890abcdef','2025-07-25 05:00:00.000000','2025-07-25 07:00:00.000000','',13,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4c48-8a44-15cbe4db1abb','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','a9b2c3d4-e5f6-4a7b-8901-bcde23456789','2025-07-25 08:00:00.000000','2025-07-25 10:00:00.000000','',14,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4c74-858e-ef3160867afc','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','abcdef98-7654-3210-fedc-ba9876543210','2025-07-25 11:00:00.000000','2025-07-25 13:00:00.000000','',15,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4c9a-8a7c-0b2b1ed05b31','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','b0c3d4e5-f6a7-4b8c-9012-cdef34567890','2025-07-25 14:00:00.000000','2025-07-25 16:00:00.000000','',16,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4cbb-8dba-879fd482a11a','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','c1d4e5f6-a7b8-4c9d-0123-def456789012','2025-07-25 17:00:00.000000','2025-07-25 19:00:00.000000','',17,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0),('08ddc9e4-6366-4cde-8d8e-3e49f988cda6','08ddc9de-e54f-4b75-8b3f-5603fcf428bb','d2e5f6a7-b8c9-4d0e-1234-efa567890123','2025-07-25 20:00:00.000000','2025-07-25 22:00:00.000000','',18,0,0,0,0,'2025-07-24 20:42:46.000000','2025-07-24 20:42:46.000000',NULL,'','','',1,0);
/*!40000 ALTER TABLE `trip_plan_locations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trip_plans`
--

DROP TABLE IF EXISTS `trip_plans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trip_plans` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `start_date` datetime(6) NOT NULL,
  `end_date` datetime(6) NOT NULL,
  `image_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_trip_plan_version_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_trip_plans_user_id` (`user_id`),
  CONSTRAINT `FK_trip_plans_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trip_plans`
--

LOCK TABLES `trip_plans` WRITE;
/*!40000 ALTER TABLE `trip_plans` DISABLE KEYS */;
INSERT INTO `trip_plans` VALUES ('08ddc9de-e54f-4b75-8b3f-5603fcf428bb','string','string','2025-07-23 00:00:00.000000','2025-07-25 00:00:00.000000','https://img.freepik.com/free-vector/dark-gradient-background-with-copy-space_53876-99548.jpg?semt=ais_hybrid&w=740','9b15535f-c6a9-433b-a130-9e3acf468f4d','','2025-07-23 11:48:52.202983','2025-07-23 12:59:05.928472',NULL,'0f92840b-fc82-4dd2-aae1-7b35981d1e0e','0f92840b-fc82-4dd2-aae1-7b35981d1e0e','',1,0);
/*!40000 ALTER TABLE `trip_plans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_announcements`
--

DROP TABLE IF EXISTS `user_announcements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_announcements` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `announcement_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `is_readed` tinyint(1) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_user_announcements_announcement_id` (`announcement_id`),
  KEY `IX_user_announcements_user_id` (`user_id`),
  CONSTRAINT `FK_user_announcements_announcements_announcement_id` FOREIGN KEY (`announcement_id`) REFERENCES `announcements` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_user_announcements_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_announcements`
--

LOCK TABLES `user_announcements` WRITE;
/*!40000 ALTER TABLE `user_announcements` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_announcements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_roles`
--

DROP TABLE IF EXISTS `user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_roles` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `role_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_user_roles_role_id` (`role_id`),
  KEY `IX_user_roles_user_id` (`user_id`),
  CONSTRAINT `FK_user_roles_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_user_roles_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
INSERT INTO `user_roles` VALUES ('08ddcabf-ee14-4e6c-8006-ed8d7ae617ec','9b15535f-c6a9-433b-a130-9e3acf468f4d','08ddcabf-ecfe-46fd-85ed-fa3d45413017','2025-07-24 14:39:43.834894','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0),('08ddcac0-a75b-453a-8e12-b4bde6ff2ddc','08ddcac0-a740-4e88-8adb-4b92b6424b39','08ddcabf-ed0d-4cb6-8727-0faa305f54d5','2025-07-24 14:44:54.677198','0001-01-01 00:00:00.000000',NULL,'system','system',NULL,1,0),('08ddcac0-a75b-453a-8e12-b4bde6ff2ddd','08ddcac0-a740-4e88-8adb-4b92b6424b39','08ddcabf-ed0d-4ddf-8f1e-f23cf008b6d0','2025-07-24 14:44:54.677198','0001-01-01 00:00:00.000000',NULL,'system','system',NULL,1,0);
/*!40000 ALTER TABLE `user_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email_confirmed` tinyint(1) DEFAULT NULL,
  `password_salt` varbinary(128) DEFAULT NULL,
  `password_hash` varbinary(64) DEFAULT NULL,
  `phone_number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `phone_number_confirmed` tinyint(1) DEFAULT NULL,
  `full_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `profile_picture_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `email_code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `avatar_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `sex` int NOT NULL,
  `address` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `google_id` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_email_verified` tinyint(1) DEFAULT NULL,
  `reset_token` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `reset_token_expires` datetime(6) DEFAULT NULL,
  `verification_token` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `verification_token_expires` datetime(6) DEFAULT NULL,
  `lockout_end` datetime(6) DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('08ddcac0-a740-4e88-8adb-4b92b6424b39','travelogue.fpt@gmail.com',NULL,_binary '|P0|0\Ê)aJ\ëX60<\õ×\êÁd\áNÁZ4\î\Í\r\ÔH©m\Äg ¡\Å¨	fsª¯Ö¢¢/gì³³.\Ù\Ý\ìz×²\É-Z!q\Å\Î3B6¸°X\Þ\ï\÷6l-\ãj­2¯\ÓwXXBV_\ãCM©d,\Û&\Ã\\¡>ZÝ¨Õ­`1',_binary '4ú<e\\¬6^l\ì=p\Ût.c\Ý)#®8@H\\üP]Ò:\ß}ý\Ô[>h\'\ðÃ\Ï*ß·c\ÍN?',NULL,NULL,'string',NULL,NULL,NULL,0,NULL,NULL,NULL,'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjA4ZGRjYWMwLWE3NDAtNGU4OC04YWRiLTRiOTJiNjQyNGIzOSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMDhkZGNhYzAtYTc0MC00ZTg4LThhZGItNGI5MmI2NDI0YjM5IiwianRpIjoiZThlODE3YjAtNGEwNC00MjM5LWJiOGMtM2UwNjc1MGRiNjNhIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImlzUmVmcmVzaFRva2VuIjoidHJ1ZSIsImV4cCI6MTc1Mzk3MzA5NiwiaXNzIjoiVHJhdmVsb2d1ZV9BUElfbG9jYWxob3N0IiwiYXVkIjoiVHJhdmVsb2d1ZV9BUElfbG9jYWxob3N0In0.nK_3IPl_vqEExkcDx4qEV_bdAvk-Wrwp6As1Pa0nR30','2025-07-31 14:44:56.108162','eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjA4ZGRjYWMwLWE3NDAtNGU4OC04YWRiLTRiOTJiNjQyNGIzOSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMDhkZGNhYzAtYTc0MC00ZTg4LThhZGItNGI5MmI2NDI0YjM5IiwianRpIjoiYzk4NTllMWMtODQ4OC00MDA3LTlkYWEtNzU1MTI1ZWI1ZjUwIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTc1MzU0MTA5NiwiaXNzIjoiVHJhdmVsb2d1ZV9BUElfbG9jYWxob3N0IiwiYXVkIjoiVHJhdmVsb2d1ZV9BUElfbG9jYWxob3N0In0.3A17xLQBKMtEBWqsnkohPJrTmGodrzOZOAWb0k-HHlM','2025-07-26 14:44:56.108158',NULL,'2025-07-24 14:44:54.481720','0001-01-01 00:00:00.000000',NULL,'08ddcac0-a740-4e88-8adb-4b92b6424b39','08ddcac0-a740-4e88-8adb-4b92b6424b39',NULL,1,0),('9b15535f-c6a9-433b-a130-9e3acf468f4d','traveloguetayninh@gmail.com',1,_binary '£vP\ßg\ô~ü\ÊUF\Öp\r\ß{|\\Í\êl\\IAR{\ßaLL%\ÒY°Dü\\\rYXAÛû5\ÎXýª³=¤,7¾x¹-Ã¦u¶\óZ\Û\ÅY¸\ôj,B\Å)\Åx\n?W½\Õ+.*\÷;K·2£¡fC\×=\È\Ð9!`',_binary 'CQ\Zft\È0À/ºu!øKcXº\ñ£xwþ·s»f\÷\Ôc\Ð¬\ï\Â\ô-\rOÛV-:úh\Ñe¤\ÛS\'<',NULL,NULL,'Administrator',NULL,NULL,NULL,1,NULL,NULL,1,'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjliMTU1MzVmLWM2YTktNDMzYi1hMTMwLTllM2FjZjQ2OGY0ZCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiOWIxNTUzNWYtYzZhOS00MzNiLWExMzAtOWUzYWNmNDY4ZjRkIiwianRpIjoiYzFhZDVjZGQtMzNhOS00MWU3LTg3ZDEtYzExZDE4ZmY3YTkzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJpc1JlZnJlc2hUb2tlbiI6InRydWUiLCJleHAiOjE3NTM5NzI4MDUsImlzcyI6IlRyYXZlbG9ndWVfQVBJX2xvY2FsaG9zdCIsImF1ZCI6IlRyYXZlbG9ndWVfQVBJX2xvY2FsaG9zdCJ9.6LFoZe6OaLuu59-O5zZDJZ-UJ9I2VuVklUdzPHmMtFc','2025-07-31 14:40:05.450358','eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjliMTU1MzVmLWM2YTktNDMzYi1hMTMwLTllM2FjZjQ2OGY0ZCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiOWIxNTUzNWYtYzZhOS00MzNiLWExMzAtOWUzYWNmNDY4ZjRkIiwianRpIjoiMjdlMWM0YjYtZGY2ZC00MGNlLWI2MWYtZDM1MDA0ODRlMWM2IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3NTM1NDA4MDUsImlzcyI6IlRyYXZlbG9ndWVfQVBJX2xvY2FsaG9zdCIsImF1ZCI6IlRyYXZlbG9ndWVfQVBJX2xvY2FsaG9zdCJ9.CbVpctR-6WBHbo3s_8FmehhjPQ93oXVe3w69z4WGK_U','2025-07-26 14:40:05.450141',NULL,'2025-07-24 14:39:42.371906','0001-01-01 00:00:00.000000',NULL,NULL,NULL,NULL,1,0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wallets`
--

DROP TABLE IF EXISTS `wallets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wallets` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `balance` decimal(10,2) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_wallets_user_id` (`user_id`),
  CONSTRAINT `FK_wallets_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wallets`
--

LOCK TABLES `wallets` WRITE;
/*!40000 ALTER TABLE `wallets` DISABLE KEYS */;
/*!40000 ALTER TABLE `wallets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `withdrawal_requests`
--

DROP TABLE IF EXISTS `withdrawal_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `withdrawal_requests` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `craft_village_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `wallet_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `amount` decimal(18,2) NOT NULL,
  `request_time` datetime(6) NOT NULL,
  `status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `processed_by` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `processed_at` datetime(6) DEFAULT NULL,
  `end_date` datetime(6) DEFAULT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_withdrawal_requests_craft_village_id` (`craft_village_id`),
  KEY `IX_withdrawal_requests_user_id` (`user_id`),
  KEY `IX_withdrawal_requests_wallet_id` (`wallet_id`),
  CONSTRAINT `FK_withdrawal_requests_craft_villages_craft_village_id` FOREIGN KEY (`craft_village_id`) REFERENCES `craft_villages` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_withdrawal_requests_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_withdrawal_requests_wallets_wallet_id` FOREIGN KEY (`wallet_id`) REFERENCES `wallets` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `withdrawal_requests`
--

LOCK TABLES `withdrawal_requests` WRITE;
/*!40000 ALTER TABLE `withdrawal_requests` DISABLE KEYS */;
/*!40000 ALTER TABLE `withdrawal_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `workshop_activities`
--

DROP TABLE IF EXISTS `workshop_activities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workshop_activities` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `workshop_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `activity` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `start_time` time(6) DEFAULT NULL,
  `end_time` time(6) DEFAULT NULL,
  `notes` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `day_order` int NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_workshop_activities_workshop_id` (`workshop_id`),
  CONSTRAINT `FK_workshop_activities_workshops_workshop_id` FOREIGN KEY (`workshop_id`) REFERENCES `workshops` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `workshop_activities`
--

LOCK TABLES `workshop_activities` WRITE;
/*!40000 ALTER TABLE `workshop_activities` DISABLE KEYS */;
/*!40000 ALTER TABLE `workshop_activities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `workshop_medias`
--

DROP TABLE IF EXISTS `workshop_medias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workshop_medias` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `media_url` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `file_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `file_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `size_in_bytes` float NOT NULL,
  `is_thumbnail` tinyint(1) NOT NULL,
  `workshop_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_workshop_medias_workshop_id` (`workshop_id`),
  CONSTRAINT `FK_workshop_medias_workshops_workshop_id` FOREIGN KEY (`workshop_id`) REFERENCES `workshops` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `workshop_medias`
--

LOCK TABLES `workshop_medias` WRITE;
/*!40000 ALTER TABLE `workshop_medias` DISABLE KEYS */;
/*!40000 ALTER TABLE `workshop_medias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `workshop_schedules`
--

DROP TABLE IF EXISTS `workshop_schedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workshop_schedules` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `workshop_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `start_time` datetime(6) NOT NULL,
  `end_time` datetime(6) NOT NULL,
  `max_participant` int NOT NULL,
  `current_booked` int NOT NULL,
  `notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `adult_price` decimal(65,30) NOT NULL,
  `children_price` decimal(65,30) NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_workshop_schedules_workshop_id` (`workshop_id`),
  CONSTRAINT `FK_workshop_schedules_workshops_workshop_id` FOREIGN KEY (`workshop_id`) REFERENCES `workshops` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `workshop_schedules`
--

LOCK TABLES `workshop_schedules` WRITE;
/*!40000 ALTER TABLE `workshop_schedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `workshop_schedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `workshops`
--

DROP TABLE IF EXISTS `workshops`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workshops` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `content` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` int NOT NULL,
  `craft_village_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_time` datetime(6) NOT NULL,
  `last_updated_time` datetime(6) NOT NULL,
  `deleted_time` datetime(6) DEFAULT NULL,
  `created_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `deleted_by` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `is_active` tinyint(1) NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_workshops_craft_village_id` (`craft_village_id`),
  CONSTRAINT `FK_workshops_craft_villages_craft_village_id` FOREIGN KEY (`craft_village_id`) REFERENCES `craft_villages` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `workshops`
--

LOCK TABLES `workshops` WRITE;
/*!40000 ALTER TABLE `workshops` DISABLE KEYS */;
/*!40000 ALTER TABLE `workshops` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'travelogue'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-24 21:52:58
