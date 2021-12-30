package goodgartic.mrclean.repositories

import goodgartic.mrclean.entities.Filter
import org.springframework.data.repository.CrudRepository
import org.springframework.stereotype.Repository
import java.util.*

@Repository
interface FiltersRepository : CrudRepository<Filter, UUID>