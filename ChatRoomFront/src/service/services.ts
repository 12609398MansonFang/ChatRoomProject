import axios from "axios"
import type { LoginRequest, UserAll, Room, Message, CreateMessage, Notification, CreateNotification, UpdateRoom} from "../type/types"

const API_URL = 'http://ec2-3-106-121-130.ap-southeast-2.compute.amazonaws.com'

//---------------------------------------Message------------------------------------------//
export async function getMessages(Id: number): Promise<Message[]> {
    const response = await axios.get(`${API_URL}/message/${Id}`)
    return response.data;
}

export async function createMessage(createMessage: CreateMessage): Promise<any> {
    const response = await axios.post(`${API_URL}/message`, createMessage)
    return response.data    
}

export async function deleteMessage(Id: number): Promise<any> {
    const response = await axios.delete(`${API_URL}/message/${Id}`)
    return response.data    
}

//---------------------------------------Notification------------------------------------------//
export async function getNotification(Id: number): Promise<Notification[]> {
    const response = await axios.get<Notification[]>(`${API_URL}/notification/${Id}`);
    return response.data;
}

export async function createNotification(createNotification: CreateNotification): Promise<any> {
    const response = await axios.post(`${API_URL}/notification`, createNotification);
    return response.data;
}

export async function updateNotification(id: number, updatedUsers: number[]): Promise<Notification> {
    const response = await axios.put<Notification>(`${API_URL}/notification/${id}`, { users: updatedUsers })
    return response.data
}

//---------------------------------------Room------------------------------------------//
export async function getRooms(Id: number): Promise<Room[]> { 
    const response = await axios.get(`${API_URL}/room/${Id}`)
    return response.data;
}

export async function updateRoom(Id: number, updateRoom: UpdateRoom): Promise<any> {
    const response = await axios.put(`${API_URL}/room/${Id}`, updateRoom)
    return response.data
}

export async function createRoom(createRoom: UpdateRoom): Promise<any> {
    const response = await axios.post(`${API_URL}/room`, createRoom)
    return response.data    
}

//---------------------------------------User------------------------------------------//
// - Login using Email and Password
export async function loginUser(user: LoginRequest): Promise<any>{
    return await axios.post(`${API_URL}/user/login`, user)
}

//-------------------------------------------TESTING-----------------------------------------//

// - Export all user
export async function getUsers(): Promise<UserAll[]>{
    const response = await axios.get<UserAll[]>(`${API_URL}/user`)
    return response.data;
}