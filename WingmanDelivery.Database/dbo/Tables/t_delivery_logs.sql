CREATE TABLE [dbo].[t_delivery_logs] (
    [f_uid]          UNIQUEIDENTIFIER NOT NULL,
    [f_iid]          INT              IDENTITY (1, 1) NOT NULL,
    [f_delivery_uid] UNIQUEIDENTIFIER NULL,
    [f_status_from]  INT              NULL,
    [f_status_to]    INT              NULL,
    [f_create_date]  DATETIME         NOT NULL,
    [f_create_by]    UNIQUEIDENTIFIER NOT NULL,
    [f_update_date]  DATETIME         NULL,
    [f_update_by]    UNIQUEIDENTIFIER NULL,
    [f_delete_date]  DATETIME         NULL,
    [f_delete_by]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_delive__EAF60E98D52B9CCA] PRIMARY KEY CLUSTERED ([f_uid] ASC)
);

