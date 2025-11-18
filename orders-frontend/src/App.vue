<template>
  <div class="app-root">
    <div class="app-container">
      <header class="app-header">
        <h1 class="app-title">Generic Forms Frontend</h1>
        <p class="app-subtitle">
          Vue app talking to your ASP.NET generic /api/forms backend. Create
          orders and list/search submissions from any form type on one page.
        </p>
      </header>

      
      <section class="card">
        <h2 class="section-title">Order Form</h2>
        <p class="section-subtitle">
          Example form using the <span class="badge">orders</span> formType. On
          submit, payload is sent to <code>/api/forms/orders</code>.
        </p>

        <form @submit.prevent="handleSubmit" novalidate>
         
          <div class="form-group">
            <label for="customerName">Customer Name *</label>
            <input
              id="customerName"
              type="text"
              v-model.trim="form.customerName"
              @blur="touchField('customerName')"
            />
            <div class="error" v-if="errors.customerName">
              {{ errors.customerName }}
            </div>
          </div>

        
          <div class="form-group">
            <label for="email">Email *</label>
            <input
              id="email"
              type="email"
              v-model.trim="form.email"
              @blur="touchField('email')"
            />
            <div class="error" v-if="errors.email">
              {{ errors.email }}
            </div>
          </div>

          
          <div class="form-group">
            <label for="orderType">Order Type *</label>
            <select
              id="orderType"
              v-model="form.orderType"
              @blur="touchField('orderType')"
            >
              <option disabled value="">Please select</option>
              <option value="standard">Standard</option>
              <option value="express">Express</option>
              <option value="overnight">Overnight</option>
            </select>
            <div class="error" v-if="errors.orderType">
              {{ errors.orderType }}
            </div>
          </div>

          <div class="form-group">
            <label for="orderDate">Order Date *</label>
            <input
              id="orderDate"
              type="date"
              v-model="form.orderDate"
              @blur="touchField('orderDate')"
            />
            <div class="error" v-if="errors.orderDate">
              {{ errors.orderDate }}
            </div>
          </div>

          
          <div class="form-group">
            <label>Shipping Method *</label>
            <div class="radio-group">
              <label class="radio-option">
                <input
                  type="radio"
                  value="pickup"
                  v-model="form.shippingMethod"
                  @change="touchField('shippingMethod')"
                />
                Pickup
              </label>
              <label class="radio-option">
                <input
                  type="radio"
                  value="home"
                  v-model="form.shippingMethod"
                  @change="touchField('shippingMethod')"
                />
                Home delivery
              </label>
              <label class="radio-option">
                <input
                  type="radio"
                  value="locker"
                  v-model="form.shippingMethod"
                  @change="touchField('shippingMethod')"
                />
                Parcel locker
              </label>
            </div>
            <div class="error" v-if="errors.shippingMethod">
              {{ errors.shippingMethod }}
            </div>
          </div>

          
          <div class="form-group">
            <label>Options</label>
            <div class="checkbox-group">
              <label class="checkbox-option">
                <input type="checkbox" v-model="form.requiresSignature" />
                Requires signature on delivery
              </label>
            </div>
          </div>

         
          <div class="form-group">
            <div class="checkbox-group">
              <label class="checkbox-option">
                <input
                  type="checkbox"
                  v-model="form.acceptTerms"
                  @change="touchField('acceptTerms')"
                />
                I accept the terms and conditions *
              </label>
            </div>
            <div class="error" v-if="errors.acceptTerms">
              {{ errors.acceptTerms }}
            </div>
          </div>

          <div class="button-row">
            <button type="button" class="btn-secondary" @click="resetForm">
              Reset
            </button>

            <button type="submit" class="btn-primary" :disabled="submitting">
              {{ submitting ? "Submitting..." : "Submit order" }}
            </button>
          </div>
        </form>
      </section>

     
      <section class="card">
        <h2 class="section-title">Submitted Objects</h2>
        <p class="section-subtitle">
          List and search stored entries from different form types
          (<code>orders</code>, <code>contact</code>, etc.) via the generic
          <code>/api/forms/{formType}</code> endpoints.
        </p>

        <div class="list-controls">
          <div class="list-controls-group">
            <label for="listFormType">Form type</label>
            <input
              id="listFormType"
              type="text"
              v-model.trim="listFormType"
              @keyup.enter="fetchSubmissions(1)"
              placeholder="e.g. orders"
            />
          </div>

          <div class="list-controls-group">
            <label for="searchTerm">Search (in JSON)</label>
            <input
              id="searchTerm"
              type="text"
              v-model.trim="searchTerm"
              @keyup.enter="fetchSubmissions(1)"
              placeholder="Customer name, status, etc."
            />
          </div>

          <div class="list-controls-group">
            <label for="pageSize">Page size</label>
            <select
              id="pageSize"
              v-model.number="pageSize"
              @change="fetchSubmissions(1)"
            >
              <option :value="5">5</option>
              <option :value="10">10</option>
              <option :value="20">20</option>
            </select>
          </div>

          <div class="list-controls-group" style="align-self: flex-end">
            <button
              class="btn-secondary"
              type="button"
              @click="fetchSubmissions(1)"
            >
              Refresh
            </button>
          </div>
        </div>

        

        <div class="list-controls" v-if="listFormType === 'orders'">
          <div class="list-controls-group">
            <label for="fStatus">Order status</label>
            <select
              id="fStatus"
              v-model="filterOrderStatus"
              @change="fetchSubmissions(1)"
            >
              <option value="">Any</option>
              <option value="Pending">Pending</option>
              <option value="Shipped">Shipped</option>
              <option value="Delivered">Delivered</option>
            </select>
          </div>

          <div class="list-controls-group">
            <label for="fPriority">Priority</label>
            <select
              id="fPriority"
              v-model="filterPriority"
              @change="fetchSubmissions(1)"
            >
              <option value="">Any</option>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </div>

          <div class="list-controls-group">
            <label for="fSig">Requires signature</label>
            <select
              id="fSig"
              v-model="filterRequiresSignature"
              @change="fetchSubmissions(1)"
            >
              <option value="">Any</option>
              <option value="true">Yes</option>
              <option value="false">No</option>
            </select>
          </div>

          <div class="list-controls-group">
            <label for="fFrom">Order date from</label>
            <input
              id="fFrom"
              type="date"
              v-model="filterOrderDateFrom"
              @change="fetchSubmissions(1)"
            />
          </div>

          <div class="list-controls-group">
            <label for="fTo">Order date to</label>
            <input
              id="fTo"
              type="date"
              v-model="filterOrderDateTo"
              @change="fetchSubmissions(1)"
            />
          </div>
        </div>

        <div class="loader" v-if="listLoading">Loading entries...</div>

        <div v-if="items.length === 0 && !listLoading" class="muted">
          No entries found for <code>{{ listFormType }}</code>
          <span v-if="searchTerm"> with search "{{ searchTerm }}"</span>.
        </div>

        <div v-else class="table-wrapper">
          <table class="table">
            <thead>
              <tr>
                <th>Id</th>
                <th>Created</th>
                <th>FormType</th>
                <th>OrderStatus / Summary (from JSON)</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in items" :key="item.id">
                <td>{{ item.id }}</td>
                <td>{{ formatDateTime(item.createdAt) }}</td>
                <td>
                  <span class="badge">{{ item.formType }}</span>
                </td>
                <td>
                  <div v-if="item.data">
                    <span
                      v-if="item.data.OrderStatus"
                      :class="statusClass(item.data.OrderStatus)"
                      class="status-pill"
                    >
                      {{ item.data.OrderStatus }}
                    </span>
                    <span v-if="item.data.CustomerName">
                      &nbsp;{{ item.data.CustomerName }}
                    </span>
                  </div>
                  <div v-else class="muted">JSON parse error</div>
                  <div class="muted">
                    {{ truncateJson(item.rawData) }}
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pagination" v-if="totalCount > 0">
          <span>
            Showing {{ pageStart }}–{{ pageEnd }} of {{ totalCount }}
          </span>
          <div class="pagination-buttons">
            <button
              type="button"
              class="btn-secondary"
              :disabled="page <= 1"
              @click="fetchSubmissions(page - 1)"
            >
              Prev
            </button>
            <button
              type="button"
              class="btn-secondary"
              :disabled="pageEnd >= totalCount"
              @click="fetchSubmissions(page + 1)"
            >
              Next
            </button>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { computed, reactive, ref, onMounted } from "vue";


const baseApiUrl = import.meta.env.VITE_API_BASE_URL;

const filterOrderStatus = ref("");
const filterPriority = ref("");
const filterRequiresSignature = ref(""); // '', 'true', 'false'
const filterOrderDateFrom = ref("");
const filterOrderDateTo = ref("");

// -------- Order form state/validation --------
const form = reactive({
  customerName: "",
  email: "",
  orderType: "",
  orderDate: "",
  shippingMethod: "",
  requiresSignature: false,
  acceptTerms: false,
});

const touched = reactive({
  customerName: false,
  email: false,
  orderType: false,
  orderDate: false,
  shippingMethod: false,
  acceptTerms: false,
});

const errors = reactive({
  customerName: "",
  email: "",
  orderType: "",
  orderDate: "",
  shippingMethod: "",
  acceptTerms: "",
});

const submitting = ref(false);

const validateField = (field) => {
  switch (field) {
    case "customerName":
      if (!form.customerName) {
        errors.customerName = "Customer name is required.";
      } else if (form.customerName.length < 2) {
        errors.customerName = "Customer name must be at least 2 characters.";
      } else {
        errors.customerName = "";
      }
      break;
    case "email":
      if (!form.email) {
        errors.email = "Email is required.";
      } else {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(form.email)) {
          errors.email = "Please enter a valid email address.";
        } else {
          errors.email = "";
        }
      }
      break;
    case "orderType":
      errors.orderType = form.orderType ? "" : "Order type is required.";
      break;
    case "orderDate":
      errors.orderDate = form.orderDate ? "" : "Order date is required.";
      break;
    case "shippingMethod":
      errors.shippingMethod = form.shippingMethod
        ? ""
        : "Shipping method is required.";
      break;
    case "acceptTerms":
      errors.acceptTerms = form.acceptTerms ? "" : "You must accept the terms.";
      break;
  }
};

const touchField = (field) => {
  touched[field] = true;
  validateField(field);
};

const validateAll = () => {
  Object.keys(touched).forEach((k) => {
    touched[k] = true;
    validateField(k);
  });
  return !Object.values(errors).some((e) => e);
};

const resetForm = () => {
  form.customerName = "";
  form.email = "";
  form.orderType = "";
  form.orderDate = "";
  form.shippingMethod = "";
  form.requiresSignature = false;
  form.acceptTerms = false;

  Object.keys(errors).forEach((k) => (errors[k] = ""));
  Object.keys(touched).forEach((k) => (touched[k] = false));
  submitting.value = false;
};


const orderPayload = computed(() => ({
  CustomerName: form.customerName,
  Email: form.email,
  OrderType: form.orderType,
  OrderDate: form.orderDate,
  ShippingMethod: form.shippingMethod,
  RequiresSignature: form.requiresSignature,
  AcceptTerms: form.acceptTerms,
}));

const handleSubmit = async () => {
  if (!validateAll()) return;

  submitting.value = true;
  try {
    const res = await fetch(`${baseApiUrl}/forms/orders`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(orderPayload.value),
    });
    if (!res.ok) throw new Error("Request failed");

    
    listFormType.value = "orders";
    await fetchSubmissions(1);
    resetForm();
  } catch (err) {
    console.error(err);
  } finally {
    submitting.value = false;
  }
};


const listFormType = ref("orders");
const searchTerm = ref("");
const page = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);
const items = ref([]);
const listLoading = ref(false);

const fetchSubmissions = async (targetPage) => {
  if (!listFormType.value) return;

  listLoading.value = true;
  page.value = targetPage;

  const params = new URLSearchParams();
  params.set("page", String(page.value));
  params.set("pageSize", String(pageSize.value));
  if (searchTerm.value.trim()) {
    params.set("search", searchTerm.value.trim());
  }

  
  if (listFormType.value === "orders") {
    if (filterOrderStatus.value) {
      params.set("orderStatus", filterOrderStatus.value);
    }
    if (filterPriority.value) {
      params.set("fulfillmentPriority", filterPriority.value);
    }
    if (filterRequiresSignature.value) {
      params.set("requiresSignature", filterRequiresSignature.value);
    }
    if (filterOrderDateFrom.value) {
      params.set("orderDateFrom", filterOrderDateFrom.value);
    }
    if (filterOrderDateTo.value) {
      params.set("orderDateTo", filterOrderDateTo.value);
    }
  }

  try {
    const res = await fetch(
      `${baseApiUrl}/forms/${encodeURIComponent(listFormType.value)}?` +
        params.toString()
    );
    if (!res.ok) throw new Error("List request failed");

    const data = await res.json();
    totalCount.value = data.totalCount || 0;
    items.value = (data.items || []).map((e) => {
      
      let parsed = null;
      if (e.data && typeof e.data === "object") {
        parsed = e.data;
      } else if (e.data && typeof e.data === "string") {
        try {
          parsed = JSON.parse(e.data);
        } catch {
          parsed = null;
        }
      }
      return {
        id: e.id,
        formType: e.formType,
        createdAt: e.createdAt,
        rawData: e.data,
        data: parsed,
      };
    });
  } catch (err) {
    console.error(err);
  } finally {
    listLoading.value = false;
  }
};

const pageStart = computed(() => {
  if (totalCount.value === 0) return 0;
  return (page.value - 1) * pageSize.value + 1;
});

const pageEnd = computed(() => {
  return Math.min(page.value * pageSize.value, totalCount.value);
});

const formatDateTime = (iso) => {
  if (!iso) return "";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return iso;
  return d.toLocaleString();
};

const truncateJson = (val) => {
  const str = typeof val === "string" ? val : JSON.stringify(val);
  if (!str) return "";
  const max = 80;
  return str.length > max ? str.slice(0, max) + "…" : str;
};

const statusClass = (status) => {
  if (!status) return "";
  const s = String(status).toLowerCase();
  if (s === "pending") return "status-pending";
  if (s === "shipped") return "status-shipped";
  if (s === "delivered") return "status-delivered";
  return "";
};

// initial load
onMounted(() => {
  fetchSubmissions(1);
});
</script>
