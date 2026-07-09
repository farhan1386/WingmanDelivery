CREATE TABLE [dbo].[t_roles] (
    [f_uid]         UNIQUEIDENTIFIER NOT NULL,
    [f_iid]         INT              IDENTITY (1, 1) NOT NULL,
    [f_role_name]   NVARCHAR (100)   NULL,
    [f_description] NVARCHAR (150)   NULL,
    [f_active]      BIT              NULL,
    [f_create_date] DATETIME         NULL,
    [f_create_by]   UNIQUEIDENTIFIER NULL,
    [f_update_date] DATETIME         NULL,
    [f_update_by]   UNIQUEIDENTIFIER NULL,
    [f_delete_date] DATETIME         NULL,
    [f_delete_by]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_t_roles] PRIMARY KEY CLUSTERED ([f_uid] ASC)
);

