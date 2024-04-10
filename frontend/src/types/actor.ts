export type Actor = {
  id: string;
  type: ActorType;
  isDeleted: boolean;
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type ActorType = "ApiKey" | "System" | "User";
