CREATE TABLE [dbo].[t_menu_items] (
    [f_uid]            UNIQUEIDENTIFIER NOT NULL,
    [f_iid]            INT              IDENTITY (1, 1) NOT NULL,
    [f_restaurant_uid] UNIQUEIDENTIFIER NOT NULL,
    [f_name]           NVARCHAR (255)   NOT NULL,
    [f_price]          DECIMAL (18, 2)  NOT NULL,
    [f_create_date]    DATETIME         NOT NULL,
    [f_create_by]      UNIQUEIDENTIFIER NOT NULL,
    [f_update_date]    DATETIME         NULL,
    [f_update_by]      UNIQUEIDENTIFIER NULL,
    [f_delete_date]    DATETIME         NULL,
    [f_delete_by]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__t_menu_i__EAF60E9888C5BA2D] PRIMARY KEY CLUSTERED ([f_uid] ASC),
    CONSTRAINT [FK__t_menu_it__f_res__534D60F1] FOREIGN KEY ([f_restaurant_uid]) REFERENCES [dbo].[t_restaurants] ([f_uid])
);

