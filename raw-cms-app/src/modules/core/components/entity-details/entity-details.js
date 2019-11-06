import { epicSpinners } from '../../../../utils/spinners.js';
import { entitiesSchemaService } from '../../services/entities-schema.service.js';

const _EntityDetails = async (res, rej) => {
  const tpl = await RawCMS.loadComponentTpl(
    '/modules/core/components/entity-details/entity-details.tpl.html'
  );

  res({
    components: {
      AtomSpinner: epicSpinners.AtomSpinner,
    },
    created: function() {
      this.fetchEntityDetails();
    },
    data: () => {
      return {
        headers: [
          { text: 'Name', value: 'Name' },
          { text: 'Required', value: 'Required' },
          { text: 'Type', value: 'Type' }
        ],
        isLoading: true,
        fieldSettings: []
      };
    },
    methods: {
      fetchEntityDetails: function() {
        setTimeout(async () => {
          const res = await entitiesSchemaService.getEntityDetails(this.$route.params.id);
          this.fieldSettings = res;
          this.isLoading = false;
        }, 1000);
      }
    },
    template: tpl,
    watch: {
      $route: 'fetchData',
    },
  });
};

export const EntityDetails = _EntityDetails;
export default _EntityDetails;
