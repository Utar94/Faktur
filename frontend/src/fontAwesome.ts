import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import { faBan } from "@fortawesome/free-solid-svg-icons";

library.add(faBan);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
