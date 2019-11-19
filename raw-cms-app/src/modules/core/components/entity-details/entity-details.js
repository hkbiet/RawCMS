import { RawCmsDetailEditDef } from '../../../shared/components/detail-edit/detail-edit.js';
import { RawCmsListDef } from '../../../shared/components/list/list.js';
import { entitiesSchemaService } from '../../services/entities-schema.service.js';

const _FieldListWrapperDef = async () => {
  const rawCmsListDef = await RawCmsListDef();

  return {
    data: function() {
      return {
        apiService: entitiesSchemaService,
      };
    },
    extends: rawCmsListDef,
    methods: {
      amdRequire: require,
      deleteConfirmMsg(item) {
        return this.$t('core.entities.deleteConfirmMsgTpl', { name: item.CollectionName });
      },
      deleteSuccessMsg(item) {
        return this.$t('core.entities.deleteSuccessMsgTpl', { name: item.CollectionName });
      },
      deleteErrorMsg(item) {
        return this.$t('core.entities.deleteErrorMsgTpl', { name: item.CollectionName });
      },
    },
    props: {
      detailRouteName: {
        typ: String,
        default: 'field-list',
      },
    },
  };
};

const _EntityDetailsWrapperDef = async () => {
  const rawCmsDetailEditDef = await RawCmsDetailEditDef();

  return {
    data: function() {
      return {
        apiService: entitiesSchemaService,
      };
    },
    extends: rawCmsDetailEditDef,
  };
};

const _EntityDetailsDef = async () => {
  const detailWrapperDef = await _EntityDetailsWrapperDef();
  const listWrapperDef = await _FieldListWrapperDef();
  const tpl = await RawCMS.loadComponentTpl(
    '/modules/core/components/entity-details/entity-details.tpl.html'
  );

  return {
    components: {
      DetailWrapper: detailWrapperDef,
      ListWrapper: listWrapperDef,
    },
    props: Vue.extend(detailWrapperDef.extends.props, {
      monacoScriptOptions: {
        language: 'javascript',
        scrollBeyondLastLine: false,
      },
    }),
    methods: detailWrapperDef.extends.methods,
    template: tpl,
  };
};

const _EntityDetails = async (res, rej) => {
  const cmpDef = _EntityDetailsDef();
  res(cmpDef);
};

export const EntityDetailsDef = _EntityDetailsDef;
export const EntityDetails = _EntityDetails;
export default _EntityDetails;
