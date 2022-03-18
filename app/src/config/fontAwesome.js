import Vue from 'vue'
import { library } from '@fortawesome/fontawesome-svg-core'
import {
  faBan,
  faCarrot,
  faEdit,
  faEye,
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
  faBan,
  faCarrot,
  faEdit,
  faEye,
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
)

Vue.component('font-awesome-icon', FontAwesomeIcon)
