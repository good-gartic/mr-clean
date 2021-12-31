package goodgartic.mrclean.repositories

import goodgartic.mrclean.entities.Filter
import org.springframework.data.repository.CrudRepository
import org.springframework.stereotype.Repository

@Repository
interface FiltersRepository : CrudRepository<Filter, Int>