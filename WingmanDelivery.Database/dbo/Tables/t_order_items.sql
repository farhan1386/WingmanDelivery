CREATE TABLE [dbo].[t_order_items] (
    [f_uid]           UNIQUEIDENTIFIER NOT NULL,
    [f_iid]           INT              IDENTITY (1, 1) NOT NULL,
    [f_order_uid]     UNIQUEIDENTIFIER NOT NULL,
    [f_menu_item_uid] UNIQUEIDENTIFIER NOT NULL,
    [f_quantity]      INT              NOT NULL,
    [f_create_date]   DATETIME         NOT NULL,
    [f_create_by]     UNIQUEIDENTIFIER NOT NULL,
    [f_update_date]   DATETIME         NULL,
    [f_update_by]     UNIQUEIDENTIFIER NULL,
    [f_delete_date]   DATETIME         NULL,
    [f_delete_by]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_order___EAF60E9899D74CD0] PRIMARY KEY CLUSTERED ([f_uid] ASC),
    CONSTRAINT [FK__t_order_i__f_men__59063A47] FOREIGN KEY ([f_menu_item_uid]) REFERENCES [dbo].[t_menu_items] ([f_uid]),
    CONSTRAINT [FK__t_order_i__f_ord__5812160E] FOREIGN KEY ([f_order_uid]) REFERENCES [dbo].[t_orders] ([f_uid])
);

