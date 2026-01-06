CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `ChatRooms` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `name` longtext CHARACTER SET utf8mb4 NULL,
        `type` longtext CHARACTER SET utf8mb4 NOT NULL,
        `created_at` datetime(6) NOT NULL,
        `group_id` int NULL,
        CONSTRAINT `PK_ChatRooms` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `Messages` (
        `id` int NOT NULL AUTO_INCREMENT,
        `chatroom_id` int NOT NULL,
        `user_id` int NOT NULL,
        `text` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `created_at` datetime(6) NOT NULL,
        CONSTRAINT `PK_Messages` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `Organisations` (
        `id` int NOT NULL AUTO_INCREMENT,
        `name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Organisations` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `Roles` (
        `id` int NOT NULL AUTO_INCREMENT,
        `name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Roles` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `Users` (
        `id` int NOT NULL AUTO_INCREMENT,
        `user_name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `password` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `Groups` (
        `id` int NOT NULL AUTO_INCREMENT,
        `organisation_id` int NOT NULL,
        `name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `description` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Groups` PRIMARY KEY (`id`),
        CONSTRAINT `FK_Groups_Organisations_organisation_id` FOREIGN KEY (`organisation_id`) REFERENCES `Organisations` (`id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `ChatRoomUsers` (
        `chatroom_id` int NOT NULL,
        `user_id` int NOT NULL,
        CONSTRAINT `PK_ChatRoomUsers` PRIMARY KEY (`chatroom_id`, `user_id`),
        CONSTRAINT `FK_ChatRoomUsers_ChatRooms_chatroom_id` FOREIGN KEY (`chatroom_id`) REFERENCES `ChatRooms` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ChatRoomUsers_Users_user_id` FOREIGN KEY (`user_id`) REFERENCES `Users` (`id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `RefreshTokens` (
        `id` int NOT NULL AUTO_INCREMENT,
        `user_id` int NOT NULL,
        `token` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `created_at` datetime(6) NOT NULL,
        `expires_at` datetime(6) NOT NULL,
        `revoked_at` datetime(6) NULL,
        `replaces_token` varchar(255) CHARACTER SET utf8mb4 NULL,
        `created_by_ip` varchar(45) CHARACTER SET utf8mb4 NULL,
        `user_agent` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_RefreshTokens` PRIMARY KEY (`id`),
        CONSTRAINT `FK_RefreshTokens_Users_user_id` FOREIGN KEY (`user_id`) REFERENCES `Users` (`id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `UserRoles` (
        `role_id` int NOT NULL,
        `user_id` int NOT NULL,
        `organisation_id` int NOT NULL,
        CONSTRAINT `PK_UserRoles` PRIMARY KEY (`user_id`, `role_id`, `organisation_id`),
        CONSTRAINT `FK_UserRoles_Organisations_organisation_id` FOREIGN KEY (`organisation_id`) REFERENCES `Organisations` (`id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UserRoles_Roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `Roles` (`id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UserRoles_Users_user_id` FOREIGN KEY (`user_id`) REFERENCES `Users` (`id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE TABLE `GroupUsers` (
        `group_id` int NOT NULL,
        `user_id` int NOT NULL,
        CONSTRAINT `PK_GroupUsers` PRIMARY KEY (`group_id`, `user_id`),
        CONSTRAINT `FK_GroupUsers_Groups_group_id` FOREIGN KEY (`group_id`) REFERENCES `Groups` (`id`) ON DELETE CASCADE,
        CONSTRAINT `FK_GroupUsers_Users_user_id` FOREIGN KEY (`user_id`) REFERENCES `Users` (`id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE INDEX `IX_ChatRoomUsers_user_id` ON `ChatRoomUsers` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE INDEX `IX_Groups_organisation_id` ON `Groups` (`organisation_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE INDEX `IX_GroupUsers_user_id` ON `GroupUsers` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE INDEX `IX_RefreshTokens_user_id` ON `RefreshTokens` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE INDEX `IX_UserRoles_organisation_id` ON `UserRoles` (`organisation_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    CREATE INDEX `IX_UserRoles_role_id` ON `UserRoles` (`role_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251211204054_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251211204054_InitialCreate', '8.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215084243_SeedRoles') THEN

    INSERT INTO `Roles` (`id`, `name`)
    VALUES (1, 'Owner'),
    (2, 'Admin'),
    (3, 'Member');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215084243_SeedRoles') THEN

    CREATE UNIQUE INDEX `IX_Users_email` ON `Users` (`email`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215084243_SeedRoles') THEN

    CREATE UNIQUE INDEX `IX_Users_user_name` ON `Users` (`user_name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215084243_SeedRoles') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251215084243_SeedRoles', '8.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215091212_Invite') THEN

    CREATE TABLE `OrganisationInvites` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `OrganisationId` int NOT NULL,
        `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
        `RoleId` int NOT NULL,
        `Token` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ExpiresAt` datetime(6) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `IsUsed` tinyint(1) NOT NULL,
        CONSTRAINT `PK_OrganisationInvites` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_OrganisationInvites_Organisations_OrganisationId` FOREIGN KEY (`OrganisationId`) REFERENCES `Organisations` (`id`) ON DELETE CASCADE,
        CONSTRAINT `FK_OrganisationInvites_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215091212_Invite') THEN

    CREATE INDEX `IX_OrganisationInvites_OrganisationId` ON `OrganisationInvites` (`OrganisationId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215091212_Invite') THEN

    CREATE INDEX `IX_OrganisationInvites_RoleId` ON `OrganisationInvites` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251215091212_Invite') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251215091212_Invite', '8.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

