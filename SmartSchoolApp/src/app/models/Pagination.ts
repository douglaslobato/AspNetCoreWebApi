import { TituloComponent } from '../components/shared/titulo/titulo.component';

export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;

}

export class PaginatedResult<T>{
  result: T;
  pagination: Pagination;
}
