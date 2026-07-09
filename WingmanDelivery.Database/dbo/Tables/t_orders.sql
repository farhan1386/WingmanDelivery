CREATE TABLE [dbo].[t_orders] (
    [f_uid]            UNIQUEIDENTIFIER NOT NULL,
    [f_iid]            INT              IDENTITY (1, 1) NOT NULL,
    [f_customer_uid]   UNIQUEIDENTIFIER NOT NULL,
    [f_restaurant_uid] UNIQUEIDENTIFIER NOT NULL,
    [f_status]         INT              NOT NULL,
    [f_total_amount]   DECIMAL (18, 2)  NOT NULL,
    [f_create_date]    DATETIME         NOT NULL,
    [f_create_by]      UNIQUEIDENTIFIER NOT NULL,
    [f_update_date]    DATETIME         NULL,
    [f_update_by]      UNIQUEIDENTIFIER NULL,
    [f_delete_date]    DATETIME         NULL,
    [f_delete_by]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_orders__EAF60E98644EC8AD] PRIMARY KEY CLUSTERED ([f_uid] ASC)
);

