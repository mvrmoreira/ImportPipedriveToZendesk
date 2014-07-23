using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportPipedriveToZendesk {
    class PipedriveResponse {
        public bool success { get; set; }
        public List<PipedrivePerson> data { get; set; }
        public PipedriveAdditionalData additional_data { get; set; }
    }

    class PipedrivePerson {
        public int id { get; set; }
        public string name { get; set; }
    }

    class PipedriveAdditionalData {
        public PipedrivePagination pagination { get; set; }
    }

    class PipedrivePagination {
        public int start { get; set; }
        public int limit { get; set; }
        public bool more_items_in_collection { get; set; }
        public int next_start { get; set; }
    }
}
