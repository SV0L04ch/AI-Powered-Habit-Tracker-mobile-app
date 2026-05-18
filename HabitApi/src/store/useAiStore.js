import { create } from "zustand";
import { persist } from "zustand/middleware";
import { aiResponse } from "../services/aiResponseService.js"
import { error } from "node:console";

const useAiResponse = create(
    persist((set) => ({
        response: null,
        error: null,
        isLoading: false,

        aiResp: async (resp) => {
            set({isLoading: true})
            try {
                const data = await aiResponse(response)
                set({response: data.response, error: null, isLoading: null})
            } catch (err) {
                
            }
        }
    })
    )
)