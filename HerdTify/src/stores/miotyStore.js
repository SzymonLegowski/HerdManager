import { defineStore } from "pinia";

export const useMiotyStore = defineStore('mioty', {
    state: () => ({
        mioty: (() => {
            const stored = localStorage.getItem('mioty')
            if (stored !== null)
            {
                try {
                    return JSON.parse(stored)
                } catch (e) {
                    console.error('Błąd parsowania miotów:', e)
                }
            }
            return 0
        })(),  
        nrLochy: JSON.parse(localStorage.getItem('nrLochy')) || 0,
    }),
    actions: {
        setMiotyStore(miotyData, nrLochy) {
            this.mioty = miotyData
            this.nrLochy = nrLochy
            localStorage.setItem('mioty', JSON.stringify(this.mioty))
            localStorage.setItem('nrLochy', JSON.stringify(this.nrLochy))
        },
        clearMiotyStore() {
            this.mioty = null;
            this.nrLochy = null;
            localStorage.removeItem('mioty')
            localStorage.removeItem('nrLochy')
        }
    }
})