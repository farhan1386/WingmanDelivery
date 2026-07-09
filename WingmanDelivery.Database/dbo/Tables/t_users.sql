CREATE TABLE [dbo].[t_users] (
    [f_uid]           UNIQUEIDENTIFIER NOT NULL,
    [f_iid]           INT              IDENTITY (1, 1) NOT NULL,
    [f_fname]         NVARCHAR (100)   NOT NULL,
    [f_lname]         NVARCHAR (100)   NULL,
    [f_phone]         NVARCHAR (100)   NULL,
    [f_email]         NVARCHAR (255)   NOT NULL,
    [f_role_uid]      NVARCHAR (50)    NOT NULL,
    [f_password_hash] NVARCHAR (255)   NOT NULL,
    [f_create_date]   DATETIME         NOT NULL,
    [f_create_by]     UNIQUEIDENTIFIER NOT NULL,
    [f_update_date]   DATETIME         NULL,
    [f_update_by]     UNIQUEIDENTIFIER NULL,
    [f_delete_date]   DATETIME         NULL,
    [f_delete_by]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_users__EAF60E98C9633BA5] PRIMARY KEY CLUSTERED ([f_uid] ASC),
    CONSTRAINT [UQ__t_users__D15A0385CA67B73F] UNIQUE NONCLUSTERED ([f_email] ASC)
);

