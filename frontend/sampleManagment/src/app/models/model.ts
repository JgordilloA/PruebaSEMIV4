export const SAMPLE_TYPES = ['Sangre', 'Orina', 'Heces', 'Hisopado'];
export const TEST_CODES = ['HEM', 'COL', 'HIP', 'VIH', 'VPH'];
export const TRANSITIONS: Record<string, string[]> = {
  ['Registrada']: ['Recibida', 'Rechazada'],
  ['Recibida']: ['Procesada', 'Rechazada'],
  ['Procesada']: [],
  ['Rechazada']: [],
};

export interface SampledListItem {
  id: number;
  documentNumber: string;
  patientName: string;
  collectedAt: string;
  sampleType: string;
  status: string;
  testCount: number;
}

export interface CreatedSampleRequest {
  documentNumber: string;
  fullName: string;
  collectedAt: string;
  sampleType: string;
  tstCodes: string[];
}


export interface PagedResult<T>{
    items: T[];
    page: number,
    pageSize: number,
    totalCount: number
}

export interface CahngeStatus{
    status:string,
    rejectionReason: string | null
}