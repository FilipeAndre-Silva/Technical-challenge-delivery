export interface Contact {
    id: string;
    name: string;
    email: string;
    phone: string;
  }
  
  export interface CreateContactCommand {
    name?: string | null;
    email?: string | null;
    phone?: string | null;
  }
  
  export interface FetchContactsParams {
    SearchName?: string | null;
    IsPaged?: boolean | null;
    PageNumber?: number | null;
    PageSize?: number | null;
  }
  
  export interface PagedResult<T> {
    totalCount: number;
    totalPages: number;
    currentPage: number;
    remainingItems: number;
    remainingPages: number;
    result: T[];
  }