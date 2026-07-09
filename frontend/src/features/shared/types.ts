export interface BaseModel {
    f_uid: string; // Guid string
    f_iid: number;
    f_create_date?: string | null;
    f_create_by?: string | null;
    f_update_date?: string | null;
    f_update_by?: string | null;
    f_delete_date?: string | null;
    f_delete_by?: string | null;
    RowsCount: number;
}

export interface FilterModel {
    FilterString?: string | null;
    SearchValue?: string | null;
    OrderField?: string | null;
    Order?: string | null; 
    Skip: number;
    Take: number;
}

export interface GridDataModel<T> {
    Count: number;
    Items: T[];
}