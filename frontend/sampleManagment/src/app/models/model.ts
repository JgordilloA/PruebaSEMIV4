export const SAMPLE_TYPES = ['Sangre', 'Orina','Heces','Hisopado']



export interface SampledListItem {
    id:number,
    documentNumber: string,
    patientName:string,
    collectedAt: string,
    sampleType:string,
    status: string,
    testCount:number,
}


export interface createdSampleRequest {
    documentNumber: string,
    fullName:string,
    collectedAt: string,
    sampleType:string,
    tstCodes:string[],
}