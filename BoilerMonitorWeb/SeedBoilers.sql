CREATE TABLE dbo.BatchParameterSets (
    SetID           INT IDENTITY PRIMARY KEY,
    BatchID         INT NOT NULL, 
    ParameterName   NVARCHAR(50) NOT NULL,          -- Matches BoilerLogs column names exactly
    MinValue        DECIMAL(7,2) NULL,
    MaxValue        DECIMAL(7,2) NULL,
    Severity        NVARCHAR(20) NOT NULL DEFAULT 'Critical',
    MessageTemplate NVARCHAR(200) NULL
);

CREATE TABLE dbo.BatchValidationResults (
    ValidationID     INT IDENTITY PRIMARY KEY,
    BatchID          INT NOT NULL,
    ValidatedAt      DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    TotalMinutes     INT NOT NULL,                  -- Length of active operational processing run
    ViolationMinutes INT NOT NULL,
    ToleranceMinutes INT NOT NULL,                  -- Allowed threshold excursion envelope
    Passed           BIT NOT NULL,
    DetailsJson      NVARCHAR(MAX) NULL,            -- Serialized breakdown array
    ValidatedBy      NVARCHAR(100) NULL             -- System component identifier or User
);