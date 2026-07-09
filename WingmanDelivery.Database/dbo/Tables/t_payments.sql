CREATE TABLE [dbo].[t_payments] (
    [f_uid]         UNIQUEIDENTIFIER NOT NULL,
    [f_iid]         INT              IDENTITY (1, 1) NOT NULL,
    [f_order_uid]   UNIQUEIDENTIFIER NOT NULL,
    [f_amount]      DECIMAL (18, 2)  NOT NULL,
    [f_method]      NVARCHAR (50)    NOT NULL,
    [f_status]      NVARCHAR (50)    NOT NULL,
    [f_create_date] DATETIME         NOT NULL,
    [f_create_by]   UNIQUEIDENTIFIER NOT NULL,
    [f_update_date] DATETIME         NULL,
    [f_update_by]   UNIQUEIDENTIFIER NULL,
    [f_delete_date] DATETIME         NULL,
    [f_delete_by]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_paymen__EAF60E984CBC2202] PRIMARY KEY CLUSTERED ([f_uid] ASC),
    CONSTRAINT [FK__t_payment__f_ord__5BE2A6F2] FOREIGN KEY ([f_order_uid]) REFERENCES [dbo].[t_orders] ([f_uid])
);

