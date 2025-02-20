<script setup lang="ts">
import { ref } from 'vue';

const showModal = ref(false);

const closeModal = () => showModal.value = false;
const openModal = () => showModal.value = true;
</script>

<template>
    <!-- Footer with T&C link -->
    <footer>
        <p @click="openModal" class="link">Terms & Conditions / Privacy Policy</p>
    </footer>

    <!-- Modal Popup -->
    <div v-if="showModal" class="modal-overlay" @click="closeModal">
        <div class="modal-content" @click.stop>
            <h2>Terms & Conditions / Privacy Policy</h2>
            <ul>
                <li>We do not collect, store, or share any personal data from our users.</li>
                <li>Our services do not track your activities or gather analytics on your usage.</li>
                <li>Since we do not collect any data, there is nothing to share with third parties.</li>
                <li>We are in no way in control of your browser wallets.</li>
                <li>While we strive to provide a seamless experience, we are not responsible for any issues arising from the
                use of our services.</li>
                <li>We may update this policy from time to time, and continued use of our services constitutes acceptance of
                any changes.</li>
            </ul>
            <button @click="closeModal">Close</button>
        </div>
    </div>
</template>

<style scoped>
footer {
  text-align: center;
  margin-top: 40px;
  width: 100%;
  background: var(--color-background-mute);
  display: flex;
  justify-content: space-evenly;
}

.link {
  color: #85ff9b;
  text-decoration: underline;
  cursor: pointer;
}

.modal-overlay {
  z-index: 2;
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-content {
  background: #c7c7c7;
  padding: 20px;
  border-radius: 10px;
  text-align: center;
  max-width: 500px;
}

h2, ul, li {
  color: black;
}

button {
  margin-top: 20px;
  padding: 10px 20px;
  cursor: pointer;
}
</style>
