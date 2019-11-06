import { BaseCrudService } from '../../shared/services/base-crud-service.js';

class EntitiesSchemaService extends BaseCrudService {
  constructor() {
    super({ basePath: '/system/admin/_schema' });
  }

  async getEntityDetails(entityId) {
    const res = await this._apiClient.get('/system/admin/_schema/' + entityId);
    return res.data.data.FieldSettings;
  }
}

export const entitiesSchemaService = new EntitiesSchemaService();
export default entitiesSchemaService;
