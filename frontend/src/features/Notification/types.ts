import type { BaseModel } from "../shared/types";

export const NotificationType = {
    General: 0,
    OrderUpdate: 1,
    PaymentAlert: 2,
} as const;
export type NotificationTypeEnum = typeof NotificationType[keyof typeof NotificationType];

export const NotificationStatus = {
    Unread: 0,
    Read: 1,
} as const;
export type NotificationStatusEnum = typeof NotificationStatus[keyof typeof NotificationStatus];

export interface NotificationModel extends BaseModel {
    f_user_uid: string;
    f_message: string;
    f_type: NotificationTypeEnum;
    f_status: NotificationStatusEnum;
}