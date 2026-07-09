import type { BaseModel } from "../shared/types";

export const PaymentMethod = {
    CreditCard: 0,
    Cash: 1,
    Wallet: 2,
} as const;
export type PaymentMethodType = typeof PaymentMethod[keyof typeof PaymentMethod];

export const PaymentStatus = {
    Pending: 0,
    Completed: 1,
    Failed: 2,
    Refunded: 3,
} as const;
export type PaymentStatusType = typeof PaymentStatus[keyof typeof PaymentStatus];

export interface PaymentModel extends BaseModel {
    f_order_uid: string;
    f_amount: number;
    f_method: PaymentMethodType;
    f_status: PaymentStatusType;
}