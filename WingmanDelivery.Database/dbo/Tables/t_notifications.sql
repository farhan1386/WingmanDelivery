CREATE TABLE [dbo].[t_notifications] (
    [f_uid]         UNIQUEIDENTIFIER NOT NULL,
    [f_iid]         INT              IDENTITY (1, 1) NOT NULL,
    [f_user_uid]    UNIQUEIDENTIFIER NOT NULL,
    [f_message]     NVARCHAR (500)   NOT NULL,
    [f_type]        NVARCHAR (50)    NOT NULL,
    [f_status]      NVARCHAR (50)    NOT NULL,
    [f_create_date] DATETIME         NOT NULL,
    [f_create_by]   UNIQUEIDENTIFIER NOT NULL,
    [f_update_date] DATETIME         NULL,
    [f_update_by]   UNIQUEIDENTIFIER NULL,
    [f_delete_date] DATETIME         NULL,
    [f_delete_by]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_notifi__EAF60E984F88716D] PRIMARY KEY CLUSTERED ([f_uid] ASC)
);

