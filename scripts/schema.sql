CREATE DATABASE IF NOT EXISTS `moranbernate`
    CHARACTER SET utf8
    COLLATE utf8_unicode_ci;

USE `moranbernate`;

/*!40101 SET character_set_client = utf8 */;
DROP TABLE IF EXISTS `location`;

CREATE TABLE `location` (
  `zip` varchar(20) NOT NULL,
  `city` varchar(50) DEFAULT NULL,
  `latitude` double DEFAULT NULL,
  `longitude` double DEFAULT NULL,
  PRIMARY KEY (`zip`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `checkins`;

CREATE TABLE `checkins` (
  `CheckInId` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) NOT NULL,
  `StoreId` bigint(20) DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `Message` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
  `PictureId` bigint(20) DEFAULT NULL,
  `Json` text COLLATE utf8_unicode_ci,
  `FourSquareId` varchar(24) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Private` tinyint(1) NOT NULL,
  PRIMARY KEY (`CheckInId`),
  KEY `IX_checkins_UserId` (`UserId`,`Time`),
  KEY `IX_checkins_StoreId` (`StoreId`,`Time`),
  KEY `IX_checkins_VenueId` (`FourSquareId`,`Time`),
  KEY `IX_checkins_Time` (`Time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
