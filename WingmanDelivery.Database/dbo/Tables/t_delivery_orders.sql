CREATE TABLE [dbo].[t_delivery_orders] (
    [f_uid]            UNIQUEIDENTIFIER NOT NULL,
    [f_iid]            INT              IDENTITY (1, 1) NOT NULL,
    [f_pickup_address] NVARCHAR (255)   NULL,
    [f_status]         INT              NULL,
    [f_total_cost]     DECIMAL (18)     NULL,
    [f_create_date]    DATETIME2 (7)    NOT NULL,
    [f_create_by]      UNIQUEIDENTIFIER NOT NULL,
    [f_update_date]    DATETIME2 (7)    NULL,
    [f_update_by]      UNIQUEIDENTIFIER NULL,
    [f_delete_date]    DATETIME         NULL,
    [f_delete_by]      UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([f_uid] ASC)
);

