<script lang="ts">
import { defineComponent } from 'vue'
export default defineComponent({ name: 'Screen' })
</script>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import cookies from 'js-cookie'
import type { User, Notification, Room, UpdateRoom, Message, CreateNotification} from '../type/types'
import { loginUser, getRooms, updateRoom, createRoom, getMessages, createMessage, deleteMessage, getNotification, updateNotification, getUsers, createNotification } from '../service/services'


const emailInput = ref('')
const passwordInput = ref('')

const userCurrent = ref<User | null>(null)

const userAll = ref<User[]>([])


//----------------------------------Room---------------------------------//
const rooms = ref<Room[]>([])

const rId = ref<number>(0)

const showMemberBox = ref<number | null>(null)

const showAddInterface = ref<number | null>(null)

const showRemoveInterface = ref<number | null>(null)


const toBeAdded = ref<number[]>([])
const toBeRemoved = ref<number[]>([])

const showExistingRooms = ref<boolean>(true)

const createRoomNameInput = ref('')
const createRoomDiscInput = ref('')
const toBeCreated = ref<number[]>([])

const getUserNotInRoom = (room: Room) => {
    return userAll.value.filter(user => !room.users.includes(user.id))
}

const getUserInRoom = (room: Room) => {
    return userAll.value.filter(user => room.users.includes(user.id) && user.id !== room.admin)
}

const handleAddClick = (roomId: number) => {
    if (showAddInterface.value === roomId) {
        showAddInterface.value = null
        toBeAdded.value = []
        toBeRemoved.value = []
    } else {
        showAddInterface.value = roomId
        showRemoveInterface.value = null
        toBeAdded.value = []
        toBeRemoved.value = []
    }
}

const handleRemoveClick = (roomId: number) => {
    if (showRemoveInterface.value === roomId) {
        showRemoveInterface.value = null
        toBeAdded.value = []
        toBeRemoved.value = []

    } else {
        showRemoveInterface.value = roomId;
        showAddInterface.value = null
        toBeAdded.value = []
        toBeRemoved.value = []
    }

}

const handleAddUserToRoom = (userId: number) => {
    if (toBeAdded.value.includes(userId)) {
        toBeAdded.value = toBeAdded.value.filter(Id => Id !== userId)
        console.log(toBeAdded.value)
    } else {
        toBeAdded.value.push(userId)
        console.log(toBeAdded.value)
    }
}

const handleRemoveUserFromRoom = (userId: number) => {
    if (toBeRemoved.value.includes(userId)) {
        toBeRemoved.value = toBeRemoved.value.filter(Id => Id !== userId)
        console.log(toBeRemoved.value)
    } else {
        toBeRemoved.value.push(userId)
        console.log(toBeRemoved.value)
    }
}

const handleAddSubmit = async (room: Room) => {
    if (userCurrent.value) {
        let notification: CreateNotification = {
            type: "Room",
            text: `You have been added to >>${room.name}<<`,
            users: toBeAdded.value
        }
        try {
            const updatedMembers: number[] = []
            updatedMembers.push(...room.users, ...toBeAdded.value)
            console.log("New Users", updatedMembers)

            const updatedRoom: UpdateRoom = {
                name: room.name,
                description: room.description,
                users: updatedMembers,
                admin: room.admin
            };
            await createNotification(notification)
            await refreshNotifications()
            await updateRoom(room.id, updatedRoom)
            await refreshRooms()

        } catch (error) {
            console.error('Edit Room Error:', error)
        }
    }
}

const handleRemoveSubmit = async (room: Room) => {
    if (userCurrent.value) {
        let notification: CreateNotification = {
            type: "Room",
            text: `You have been REMOVED from>>${room.name}<<`,
            users: toBeRemoved.value
        }
        try {
            const updatedMembers: number[] = room.users.filter((user: any) => user === room.admin || !toBeRemoved.value.includes(user))
            console.log("New Users", updatedMembers)

            const updatedRoom: UpdateRoom = {
                name: room.name,
                description: room.description,
                users: updatedMembers,
                admin: room.admin
            };
            await createNotification(notification)
            await refreshNotifications()
            await updateRoom(room.id, updatedRoom)
            await refreshRooms()

        } catch (error) {
            console.error('Edit Room Error:', error)
        }
    }
}

const handleSelectMode = () => {
    showExistingRooms.value = !showExistingRooms.value
    refreshRooms();
}

const handleCreateRoomUser = (userId: number) => {
    if (toBeCreated.value.includes(userId)) {
        toBeCreated.value = toBeCreated.value.filter(Id => Id !== userId);
    } else {
        toBeCreated.value.push(userId);
    }
    console.log('Updated toBeCreated:', toBeCreated.value);
}

const handleCreateRoomSubmit = async () => {
    if (createRoomNameInput.value.trim() !== '' && createRoomDiscInput.value.trim() && toBeCreated.value.length != 0) {
        toBeCreated.value.push(userCurrent.value?.id)
        let room: UpdateRoom = {
            name: createRoomNameInput.value,
            description: createRoomDiscInput.value,
            users: toBeCreated.value,
            admin: userCurrent.value?.id
        }
        let notification: CreateNotification = {
            type: "Room",
            text: `You have been ADDED to room >>${room.name}<<`,
            users: toBeCreated.value
        }
        try {
            await createRoom(room)
            await createNotification(notification)
            await refreshNotifications()
            createRoomNameInput.value = ''
            createRoomDiscInput.value = ''
            toBeCreated.value = []
            showExistingRooms.value = true
        } catch (error) {
            console.error('Error:', error)
            alert('An error occurred during Creating room')
            createRoomNameInput.value = ''
            createRoomDiscInput.value = ''
            toBeCreated.value = []
        }
    } else {
        alert('Invalid Inputs')
        createRoomNameInput.value = ''
        createRoomDiscInput.value = ''
        toBeCreated.value = []
    }
}

const handleSelectRoom = async (RoomId: number) => {
    if (userCurrent.value) {
        try {
            rId.value  = RoomId
            resetMessages()
        } catch (error) {
            console.error('Error fetching rooms:', error)
        }
    }
}

const refreshRooms = async () => {
    if (userCurrent.value) {
        try {
            const roomsData = await getRooms(userCurrent.value.id)
            rooms.value = roomsData.map(room => ({
                id: room.id,
                name: room.name,
                description: room.description,
                users: room.users,
                admin: room.admin
            }))
        } catch (error) {
            console.error('Error fetching rooms:', error)
        }
    }
    handleAddClick(0)
    handleRemoveClick(0)
    rId.value = 0
}

//----------------------------------Messages-------------------------------------//
const messageInput = ref('')

let roomMessages = ref<Message[]>([])

const resetMessages = async () => {
    if (rId.value) {
        try {
            roomMessages.value = (await getMessages(rId.value)).map(message => ({
                id: message.id,
                text: message.text,
                userId: message.userId,
                roomId: message.roomId
            }))
        } catch (error) {
            console.error('Error fetching messages:', error)
        }
    }
}

const handleSendMessage = async () => {
    if (messageInput.value.trim() !== '') {
        let message = {
            text: messageInput.value,
            userId: userCurrent.value?.id,
            roomId: rId.value
        }
        try {
            await createMessage(message)
            await resetMessages()
            messageInput.value = ''
        } catch (error) {
            console.error('Error sending message:', error)
        }
    }
}

const handleDeleteMessage = async (messageId: number) => {
    if (userCurrent.value) {
        try {
            await deleteMessage(messageId) 
            await resetMessages()
        } catch (error) {
            console.error('Delete Message Error:', error)
        }
    }
}


//----------------------------------Notification---------------------------------//
const notificationCurrent = ref<Notification[]>([])

const handleLoginClick = async () => {
    if (emailInput.value.trim() !== '' && passwordInput.value.trim() !== '') {
        const loginInfo = { email: emailInput.value, password: passwordInput.value }
        try {
            const response = await loginUser(loginInfo)
            if (response.status == 200) {
                const userData = { id: response.data.id, name: response.data.name }
                userCurrent.value = userData
                cookies.set('user', JSON.stringify(userData), { expires: 1, secure: true, sameSite: 'Strict' })
                emailInput.value = ''
                passwordInput.value = ''
                await refreshRooms()
                await refreshNotifications()
            } else {
                alert('Credentials Incorrect or Invalid Response')
                emailInput.value = ''
                passwordInput.value = ''
            }
        } catch (error) {
            console.error('Login Error:', error)
            alert('An error occurred during login')
            emailInput.value = ''
            passwordInput.value = ''
        }
    } else {
        alert('Invalid Inputs')
        emailInput.value = ''
        passwordInput.value = ''
    }
}

const refreshNotifications = async () => {
    if (userCurrent.value) {
        try {
            const notifications = await getNotification(userCurrent.value.id)
            notificationCurrent.value = notifications.map(not => ({
                id: not.id,
                type: not.type,
                text: not.text,
                users: not.users
            }))
        } catch (error) {
            console.error('Error fetching notifications:', error)
        }
    }
}

const handleNotificationClick = async (id: number) => {
    if (userCurrent.value) {
        try {
            const notification = notificationCurrent.value.find((notification: any) => notification.id === id)

            if (notification) {
                // Filter out the current user's ID from the notification users
                const updatedUsers = notification.users.filter((userId: number) => userId !== userCurrent.value!.id)

                // Update the notification on the server
                await updateNotification(id, updatedUsers)

                // Refresh the notifications list to get the latest data
                await refreshNotifications()
            }

        } catch (error) {
            console.error('Edit Notification Error:', error)
        }
    }
}



onMounted(() => {
    const storedUser = cookies.get('user')
    if (storedUser) {
        userCurrent.value = JSON.parse(storedUser)
    }
})

onMounted(async () => {
    if (userCurrent.value) {
        try {
            const notifications = await getNotification(userCurrent.value.id)
            notificationCurrent.value = notifications.map(not => ({
                id: not.id,
                type: not.type,
                text: not.text,
                users: not.users
            }))
        } catch (error) {
            console.error('Error fetching notifications:', error)
        }
    }
})

onMounted(async () => {
    if (userCurrent.value) {
        try {
            rooms.value = (await getRooms(userCurrent.value.id)).map(room => ({
                id: room.id,
                name: room.name,
                description: room.description,
                users: room.users,
                admin: room.admin
            }));
        } catch (error) {
            console.error('Error fetching rooms:', error);
        }
    }
})

onMounted(async () => {
    if (userCurrent.value) {
        try {
            userAll.value = (await getUsers()).map(user => ({
                id: user.id,
                name: user.name
            }))
        } catch (error) {
            console.error('Error fetching users:', error);
        }
    }
})




















</script>

<template>
    <div class="Screen w-full h-full flex flex-col items-center">
        <div class="Header w-full h-[10%] flex justify-between items-center px-10 bg-slate-300">
            <h1 class="HeaderName text-2xl font-bold font-serif text-sky-600">
                CHAT ROOM
            </h1>
            <div class="LogInInterface flex space-x-2">
                <h1>Email:</h1>
                <input class="px-2" v-model="emailInput" type="email" placeholder="Email...">
                <h1>Password:</h1>
                <input class="px-2" v-model="passwordInput" type="password" placeholder="Password...">
                <button @click="handleLoginClick">LOGIN</button>
            </div>
        </div>

        <div class="Body w-full h-[80%] flex flex-col">
            <div class="w-full h-[10%] flex flex-col justify-center px-10">
                <h1 class="font-serif font-bold text-2xl">Welcome {{ userCurrent?.id }} - {{ userCurrent?.name }}</h1>
            </div>

            <div class="w-full h-[80%] flex-grow flex justify-center items-center space-x-4">

                <div class="Room w-[25%] h-[90%] flex flex-col rounded-2xl bg-slate-100">
                    <div class="RoomHeader w-[100%] h-[5%] flex justify-center items-center p-8">
                        <h1 class="font-serif font-bold">Rooms</h1>
                    </div>
                    <div class="RoomRefresh w-[100%] flex justify-center flex-col p-2">
                        <button class="bg-slate-50 hover:text-slate-400 border-b-2" @click="refreshRooms">Refresh Rooms</button>
                    </div>

                    <div class="RoomBodyExisting w-full flex flex-col justify-start p-2 flex-grow overflow-y-scroll" v-if="showExistingRooms">
                        <!--Get Rooms -->
                        <div v-for="room in rooms" :key="room.id">
                            <button class="w-[100%] px-2 bg-slate-50 hover:bg-slate-100 border-b-2" :class="{'border-sky-500 border-2': rId === room.id}"
                                    v-if="room.users.includes(userCurrent?.id)"
                                    @click="handleSelectRoom(room.id)"
                                    >
                                    <div class="w-[100%]">
                                        <h1 class="flex font-bold">{{ room.name }}</h1>
                                        <p class="flex">{{ room.description }}</p>
                                    </div>

                                    <div class="w-full">
                                        <div class="w-full flex relative">

                                            <div class="w-full flex justify-start"
                                                v-if="room.admin === userCurrent?.id">
                                                <div class="w-full flex justify-between">
                                                    <div class="text-xs flex items-center font-semibold text-slate-800 hover:text-slate-400"
                                                        @click="handleAddClick(room.id)">Add</div>
                                                    <div class="text-xs flex items-center font-semibold text-slate-800 hover:text-slate-400"
                                                        @click="handleRemoveClick(room.id)">Remove</div>
                                                </div>
                                            </div>

                                            <div class="w-full flex justify-end">
                                                <div class="text-xs flex items-center font-semibold text-slate-800 hover:text-slate-400"
                                                    @mouseover="showMemberBox = room.id"
                                                    @mouseleave="showMemberBox = null">
                                                    Members
                                                </div>

                                                <div v-if="showMemberBox === room.id"
                                                    class="absolute bottom-full right-0 bg-slate-300 p-1">
                                                    <div class="flex flex-col text-xs">
                                                        <div v-for="user in userAll.filter(user => room.users.includes(user.id))"
                                                            :key="user.id">
                                                            <p class="text-blue-500" v-if="user.id == room.admin">{{
                                                                user.name }}</p>
                                                            <p v-else>{{ user.name }}</p>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>

                                        <div class="flex flex-col w-full text-md" v-if="showAddInterface === room.id">
                                            <p class="flex text-xs font-bold pt-2"> To Add:</p>
                                            <div class="w-full" v-for="user in getUserNotInRoom(room)" :key="user.id">
                                                <div class=" hover:bg-sky-200">
                                                    <button class="w-full flex text-xs bg-sky-200" v-if="toBeAdded.includes(user.id)" @click="handleAddUserToRoom(user.id)"> > {{ user.name}}</button>
                                                    <button class="w-full flex text-xs" v-else @click="handleAddUserToRoom(user.id)"> > {{ user.name}}</button>
                                                </div>
                                            </div>
                                            <button
                                                class="flex w-full text-xs font-bold pt-2 justify-center hover:text-slate-400"
                                                @click="handleAddSubmit(room)">Submit</button>
                                        </div>

                                        <div class="flex flex-col w-full text-md"
                                            v-if="showRemoveInterface === room.id">
                                            <p class="flex text-xs font-bold pt-2"> To Remove:</p>
                                            <div class="w-full" v-for="user in getUserInRoom(room)" :key="user.id">
                                                <div class=" hover:bg-sky-200">
                                                    <button class="w-full flex text-xs bg-sky-200"
                                                        v-if="toBeRemoved.includes(user.id)"
                                                        @click="handleRemoveUserFromRoom(user.id)"> > {{ user.name
                                                        }}</button>
                                                    <button class="w-full flex text-xs" v-else
                                                        @click="handleRemoveUserFromRoom(user.id)"> > {{ user.name
                                                        }}</button>
                                                </div>
                                            </div>
                                            <button
                                                class="flex w-full text-xs font-bold pt-2 justify-center hover:text-slate-400"
                                                @click="handleRemoveSubmit(room)">Submit</button>
                                        </div>
                                    </div>
                            </button>
                        </div>
                    </div>

                    <div class="RoomBodyCreating w-full h-full flex flex-col justify-start p-2 flex-grow" v-else>
                        <div class="w-full flex flex-col space-y-2 h-full bg-slate-200 p-2">
                            <div class="w-full">
                                <h1>Name:</h1>
                                <input class="w-full mb-2 px-2" v-model="createRoomNameInput">
                                <h1>Description:</h1>
                                <input class="w-full mb-2 px-2" v-model="createRoomDiscInput">
                            </div>

                            <div class="w-full h-full">
                                <h1>Users</h1>
                                <div class="w-full bg-slate-50 flex-grow overflow-y-scroll">
                                    <div v-for="user in userAll.filter(user => user.id !== userCurrent?.id)" :key="user.id">
                                        <div class="hover:bg-sky-200">
                                            <button class="w-full flex  border-b-2 border-slate-100 bg-sky-200"
                                                v-if="toBeCreated.includes(user.id)"
                                                @click="handleCreateRoomUser(user.id)"> > {{ user.name }}</button>
                                            <button class="w-full flex border-b-2 border-slate-50" v-else
                                                @click="handleCreateRoomUser(user.id)"> > {{ user.name }} </button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <button class="w-full h-[10%] bg-slate-50 hover:text-slate-400"
                                @click="handleCreateRoomSubmit()">Submit</button>
                        </div>
                    </div>


                    <div class="RoomButtons w-[100%] flex justify-center flex-col p-2">
                        <button class="bg-slate-50 hover:text-slate-400 border-b-2" @click="handleSelectMode"
                            v-if="showExistingRooms">Create New Room</button>
                        <button class="bg-slate-50 hover:text-slate-400 border-b-2" @click="handleSelectMode"
                            v-else>Existing Rooms</button>
                    </div>

                </div>

                <div class="Messages w-[40%] h-[90%] flex flex-col rounded-2xl bg-slate-100">
                    <div class="MessagesHeader w-full h-12 flex justify-center items-center p-4">
                        <h1 class="font-serif font-bold">Messages</h1>
                    </div>
                    <div class="MessageBody flex flex-col flex-grow overflow-hidden">
                        <div class="MessageDisplay flex flex-col flex-grow space-y-2 overflow-y-auto p-2 bg-slate-50">
                            <div v-for="message in roomMessages" :key="message.id">                   
                                <div class="flex justify-end space-x-2" v-if="message.userId == userCurrent?.id">

                                    <div class="flex flex-col">
                                        <div v-for="user in userAll" :key="user.id">
                                            <p class="text-sm" v-if="user.id === message.userId">{{ user.name }}</p>
                                            <p class="text-sm" v-else-if="message.userId === null">{{ user.name }}</p>
                                        </div>                               
                                        <p class="p-1 bg-green-200">{{ message.text }} - {{ message.userId }}</p>
                                        <div class="flex justify-end">
                                            <button class="text-xs hover:text-slate-400" @click="handleDeleteMessage(message.id)">delete</button>
                                        </div>                                   
                                    </div>

                                </div>

                                <div class="flex justify-start" v-else>

                                    <div class="flex flex-col">
                                        <div class="flex justify-end" v-for="user in userAll" :key="user.id">
                                            <p class="text-sm" v-if="user.id === message.userId">{{ user.name }}</p>
                                            <p class="text-sm" v-else-if="message.userId === null">{{ user.name }}</p>
                                        </div>
                                        <p class="p-1 bg-red-200">{{ message.text }} - {{ message.userId }}</p>                          
                                    </div>

                                </div>
                            </div>
                        </div>
                        <div class="MessageInput h-[12%] flex items-center space-x-2 p-2">
                            <input class="w-[80%] px-2" v-model="messageInput" placeholder="Type Something......." :disabled="rId === 0">
                            <button class="w-[20%] px-2 hover:text-slate-400 disabled:text-slate-400" :disabled="rId === 0" @click="handleSendMessage">Send</button>
                        </div>
                    </div>
                </div>


                <div class="Notification w-[25%] h-[90%] rounded-2xl bg-slate-100">
                    <div class="NotificationHeader w-[100%] h-[5%] flex justify-center items-center p-8">
                        <h1 class="font-serif font-bold">Notifications</h1>
                    </div>
                    <div class="NotificationBody w-[100%] flex flex-col justify-center p-2">
                        <button class="w-[100%] px-2 bg-slate-50 hover:bg-slate-100 border-b-2"
                            v-for="notification in notificationCurrent" :key="notification.id"
                            @click="handleNotificationClick(notification.id)">
                            <h1 class="flex font-bold">{{ notification.type }}</h1>
                            <p class="flex">{{ notification.text }}</p>
                        </button>
                    </div>
                </div>

            </div>
        </div>
    </div>
</template>
