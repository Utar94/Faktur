import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCarrot,
  faChevronLeft,
  faEdit,
  faFlag,
  faHome,
  faKey,
  faPlus,
  faRobot,
  faRotate,
  faSackDollar,
  faSave,
  faStore,
  faTrash,
  faUser,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCarrot,
  faChevronLeft,
  faEdit,
  faHome,
  faFlag,
  faKey,
  faPlus,
  faRobot,
  faRotate,
  faSackDollar,
  faSave,
  faStore,
  faTrash,
  faUser,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
