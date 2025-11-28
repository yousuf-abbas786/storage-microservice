USE StorageService;
GO

-- Create test user: admin / admin123
-- Password hash generated using SHA256
INSERT INTO Users (Id, Username, Email, PasswordHash, CreatedAt)
VALUES (
    NEWID(),
    'admin',
    'admin@storageservice.com',
    'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', -- SHA256 hash of "admin123"
    GETUTCDATE()
);
GO

SELECT * FROM Users;
GO

