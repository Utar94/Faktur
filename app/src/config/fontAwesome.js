import Vue from 'vue'
import { library } from '@fortawesome/fontawesome-svg-core'
import {
  faArrowUp,
  faBan,
  faCarrot,
  faDollarSign,
  faEdit,
  faEye,
  faFileInvoiceDollar,
  faFlag,
  faHome,
  faKey,
  faPaperPlane,
  faPlus,
  faSave,
  faSearch,
  faShoppingCart,
  faSignInAlt,
  faSignOutAlt,
  faStore,
  faSyncAlt,
  faTasks,
  faTrashAlt,
  faUser
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

library.add(
  faArrowUp,
  faBan,
  faCarrot,
  faDollarSign,
  faEdit,
  faEye,
  faFlag,
  faHome,
  faFileInvoiceDollar,
  faKey,
  faPaperPlane,
  faPlus,
  faSave,
  faSearch,
  faShoppingCart,
  faSignInAlt,
  faSignOutAlt,
  faStore,
  faSyncAlt,
  faTasks,
  faTrashAlt,
  faUser
)

Vue.component('font-awesome-icon', FontAwesomeIcon)
