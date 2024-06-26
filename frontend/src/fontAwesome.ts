import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowDown,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUp,
  faArrowUpRightFromSquare,
  faBan,
  faCarrot,
  faChevronLeft,
  faDollarSign,
  faEdit,
  faEye,
  faFileInvoiceDollar,
  faFlag,
  faHome,
  faKey,
  faPercent,
  faPlus,
  faRobot,
  faRotate,
  faSackDollar,
  faSave,
  faShoppingCart,
  faStore,
  faTimes,
  faTrash,
  faUser,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowDown,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUp,
  faArrowUpRightFromSquare,
  faBan,
  faCarrot,
  faChevronLeft,
  faDollarSign,
  faEdit,
  faEye,
  faFileInvoiceDollar,
  faHome,
  faFlag,
  faKey,
  faPercent,
  faPlus,
  faRobot,
  faRotate,
  faSackDollar,
  faSave,
  faShoppingCart,
  faStore,
  faTimes,
  faTrash,
  faUser,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
