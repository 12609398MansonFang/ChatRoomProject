<script lang="ts">
import { defineComponent } from 'vue'
export default defineComponent({ name: 'Screen' })
</script>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import cookies from 'js-cookie'
import type { User , Notification, Room} from '../type/types'
import { loginUser,getRooms, getNotification, updateNotification, getUsers} from '../service/services'


const emailInput = ref('')
const passwordInput = ref ('')

const userCurrent = ref<User | null>(null)

const userAll = ref<User[]>([])


//----------------------------------Room---------------------------------//
const rooms = ref<Room[]>([])

const showMemberBox = ref<number | null>(null)

const showAddInterface = ref<number | null>(null)

const showRemoveInterface = ref<number | null>(null)

const toBeRemoved = ref<number[]>([])
const toBeAdded = ref<number[]>([])


const getUserNotInRoom = (room: Room) => {
    return userAll.value.filter(user => !room.users.includes(user.id)) 
}

const getUserInRoom = (room: Room) => {
    return userAll.value.filter(user => room.users.includes(user.id) && user.id !== room.admin)
}

const handleAddClick = (roomId: number) => {
    if (showAddInterface.value === roomId) {
        showAddInterface.value = null
        toBeRemoved.value = []
        toBeAdded.value = []
    } else {
        showAddInterface.value = roomId
        showRemoveInterface.value = null
        toBeRemoved.value = []
        toBeAdded.value = []
    }
}

const handleRemoveClick = (roomId: number) => {
    if (showRemoveInterface.value === roomId) {
        showRemoveInterface.value = null
        toBeRemoved.value = []
        toBeAdded.value = []
    } else {
        showRemoveInterface.value = roomId;
        showAddInterface.value = null
        toBeRemoved.value = []
        toBeAdded.value = []
    }
    
}

const handleAddUserToRoom = (userId: number) => {
    if (toBeAdded.value.includes(userId)){
        toBeAdded.value = toBeAdded.value.filter(Id => Id !==userId)
        console.log(toBeAdded.value)
    } else {
        toBeAdded.value.push(userId)
        console.log(toBeAdded.value)
    }
}

const handleRemoveUserFromRoom = (userId: number) => {
    if (toBeRemoved.value.includes(userId)){
        toBeRemoved.value = toBeRemoved.value.filter(Id => Id !==userId)
        console.log(toBeRemoved.value)
    } else {
        toBeRemoved.value.push(userId)
        console.log(toBeRemoved.value)
    }
}


const handleAddSubmit = () => {

}

const handleRemoveSubmit = () => {

}

//----------------------------------Notification---------------------------------//
const notificationCurrent = ref<Notification[]>([])

const handleLoginClick = async () => {
    if (emailInput.value.trim() !== '' && passwordInput.value.trim() !== '') {
        const loginInfo = { email: emailInput.value, password: passwordInput.value }
        try {
            const response = await loginUser(loginInfo)
            if (response.status == 200){
                const userData = {id: response.data.id, name: response.data.name}
                userCurrent.value = userData
                cookies.set('user', JSON.stringify(userData), { expires: 1, secure: true, sameSite: 'Strict' })
                emailInput.value = ''
                passwordInput.value = ''
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
                <input class="px-2" v-model="passwordInput" placeholder="Password...">
                <button @click="handleLoginClick">LOGIN</button>
            </div>
        </div>

        <div class="Body w-full h-full flex flex-col">
            <div class="w-full h-[10%] flex flex-col justify-center px-10">
                <h1 class="font-serif font-bold text-2xl">Welcome {{userCurrent?.id}} - {{ userCurrent?.name }}</h1>
            </div>

            <div class="w-full h-[80%] flex-grow flex justify-center items-center space-x-4">

                <div class="Room w-[25%] h-[90%] rounded-2xl bg-slate-100">
                    <div class="RoomHeader w-[100%] h-[5%] flex justify-center items-center p-8">
                        <h1 class="font-serif font-bold">Rooms</h1>
                    </div>

                    <div class="RoomBody w-[100%] flex flex-col justify-center p-2">
                        <!--Get Rooms -->
                        <div v-for="room in rooms" :key="room.id">
                            <button class="w-[100%] px-2 bg-slate-50 hover:bg-slate-100 border-b-2" v-if="room.users.includes(userCurrent?.id)">
                                <div class="w-[100%]">
                                    <h1 class="flex font-bold">{{ room.name }}</h1>
                                    <p class="flex">{{ room.description }}</p>
                                    <p class="flex">{{ room.users }}</p>
                                </div>

                                <div class="w-full" v-if="room.admin === userCurrent?.id">
                                    <div class="w-[100%] flex justify-between relative">
                                        <div class="text-xs flex items-center font-semibold text-slate-800 hover:text-slate-400" @click="handleAddClick(room.id)">
                                            Add
                                        </div>
                                        <div class="text-xs flex items-center font-semibold text-slate-800 hover:text-slate-400" @click="handleRemoveClick(room.id)">
                                            Remove
                                        </div>
                                        <div class="text-xs flex items-center font-semibold text-slate-800 hover:text-slate-400" @mouseover="showMemberBox = room.id" @mouseleave="showMemberBox = null">
                                            Members
                                        </div>
                                        <div v-if="showMemberBox === room.id" class="absolute bottom-full right-0 bg-slate-300 p-1">
                                            <div class="flex flex-col text-xs">
                                                <p v-for="user in userAll.filter(user => room.users.includes(user.id) && user.id !== userCurrent?.id)" :key="user.id">
                                                    {{ user.name }}
                                                </p>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="flex flex-col w-full text-md" v-if="showAddInterface === room.id">
                                        <p class="flex text-xs font-bold pt-2"> To Add:</p>
                                        <div class="w-full" v-for="user in getUserNotInRoom(room)" :key="user.id">
                                            <div class=" hover:bg-sky-200" >
                                                <button class="w-full flex text-xs bg-sky-200" v-if="toBeAdded.includes(user.id)" @click="handleAddUserToRoom(user.id)"> > {{ user.name }}</button>
                                                <button class="w-full flex text-xs" v-else @click="handleAddUserToRoom(user.id)"> > {{ user.name }}</button>
                                            </div>
                                        </div>
                                        <button class="flex w-full text-xs font-bold pt-2 justify-center">Submit</button>                                    
                                    </div>
                                    
                                    <div class="flex flex-col w-full text-md" v-if="showRemoveInterface === room.id">
                                        <p class="flex text-xs font-bold pt-2"> To Remove:</p>
                                        <div class="w-full" v-for="user in getUserInRoom(room)" :key="user.id">
                                            <div class=" hover:bg-sky-200" >
                                                <button class="w-full flex text-xs bg-sky-200" v-if="toBeRemoved.includes(user.id)" @click="handleRemoveUserFromRoom(user.id)"> > {{ user.name }}</button>
                                                <button class="w-full flex text-xs" v-else @click="handleRemoveUserFromRoom(user.id)"> > {{ user.name }}</button>
                                            </div>
                                        </div>
                                        <button class="flex w-full text-xs font-bold pt-2 justify-center">Submit</button>
                                    </div>
                                </div>
                            </button>
                        </div>
                        <!--Create Rooms -->
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
                            <h1 class="flex font-bold">{{notification.type}}</h1>
                            <p class="flex">{{notification.text}}</p>
                        </button>
                    </div>
                </div>

            </div>
        </div>
    </div>
</template>

