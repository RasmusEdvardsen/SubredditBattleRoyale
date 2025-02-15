import axios from "axios";

import type { BackendData } from "@/types";
import type { Ref } from "vue";

export const fetchData = async (backendData: Ref<BackendData | undefined, BackendData | undefined>) => {
    try {
        const response = await axios.get<BackendData>(import.meta.env.VITE_BACKEND_URI);
        backendData.value = response.data;
    } catch (error) {
        console.error(error);
    }
};