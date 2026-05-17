export interface Party {
  partyId: number;
  partyType: string; // "Individual" | "Organization"
  givenName?: string | null;
  familyName?: string | null;
  organizationName?: string | null;
  displayName?: string | null; // computed on server or DB
  email?: string | null;
  phone?: string | null;
  address1?: string | null;
  address2?: string | null;
  city?: string | null;
  stateProvince?: string | null;
  postalCode?: string | null;
  country?: string | null;
  isArchived: boolean;
  createdAt: string; // ISO date
  modifiedAt?: string | null; // ISO date or null
  caseParties?: CaseParty[]; // navigation
}

export interface Case {
  caseId: number;
  caseNumber: string;
  caseTitle?: string | null;
  status: string;  // has a default so will not be null: Allowed: Intake, Active, Pending, Closed
  matterType?: string | null;
  practiceArea?: string | null;
  court?: string | null;
  jurisdiction?: string | null;
  outcome?: string | null;
  settlementAmount?: number | null; // decimal -> number
  caseCreatedAt: string; // ISO date
  caseModifiedAt?: string | null;
  isSealed: boolean;
  caseParties?: CaseParty[];
}

export interface CaseParty {
  casePartyId: number;
  caseId: number;
  partyId: number;
  partyRole: string;
  isPrimaryContact: boolean;
  isClient: boolean;
  isAdverse: boolean;
  billingResponsibility?: string | null;
  representationStartDate?: string | null; // ISO date (date-only)
  representationEndDate?: string | null;
  notes?: string | null;
  createdAt: string; // ISO date
  modifiedAt?: string | null;
  party?: Party;
  case?: Case;
}
