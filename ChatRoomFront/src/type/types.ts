// Message
export interface Message {
    id: any
    text: any | null
    userId: any | null
    roomId: any | null
}

export interface CreateMessage {
    text: any | null
    userId: any | null
    roomId: any | null
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

export interface CreateNotification {
    type: any | null
    text: any | null
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

export interface UpdateRoom {
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

