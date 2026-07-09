import { type BaseModel } from "../shared/types";

export interface UserModel extends BaseModel {
    f_fname: string;
    f_lname: string;
    f_phone: string;
    f_email: string;
    f_password_hash: string;
    f_role_uid: string;
    f_role_name?: string | null;
}
