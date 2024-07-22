// Message
export interface Messages {
    Id: any
    Text: any | null
    UserId: any | null
    RoomId: any | null
}

// Notification
export interface Notification {
    id: any
    type: any | null
    text: any | null
    users: any | null
}

export interface UpdateNotification {
    users: any | null
}

// Room 
export interface Room {
    id: any
    name: any | null
    description: any | null
    users: any | null
    admin: any | null
}

//User
export interface User {
    id: any
    name: any | null
}

export interface LoginRequest {
    email: any | null
    password: any | null
}

export interface UserAll {
    id: any
    name: any | null
    email: any | null
    password: any | null
}

