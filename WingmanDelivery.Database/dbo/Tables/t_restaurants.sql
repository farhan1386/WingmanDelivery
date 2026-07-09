CREATE TABLE [dbo].[t_restaurants] (
    [f_uid]         UNIQUEIDENTIFIER NOT NULL,
    [f_iid]         INT              IDENTITY (1, 1) NOT NULL,
    [f_name]        NVARCHAR (255)   NOT NULL,
    [f_address]     NVARCHAR (255)   NOT NULL,
    [f_contact]     NVARCHAR (50)    NULL,
    [f_create_date] DATETIME         NOT NULL,
    [f_create_by]   UNIQUEIDENTIFIER NOT NULL,
    [f_update_date] DATETIME         NULL,
    [f_update_by]   UNIQUEIDENTIFIER NULL,
    [f_delete_date] DATETIME         NULL,
    [f_delete_by]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_restau__EAF60E982DAC351E] PRIMARY KEY CLUSTERED ([f_uid] ASC)
);

