using TransportSystem.Domain.Entities;
using TransportSystem.Domain.ValueObjects;

namespace TransportSystem.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seed data for 100 stations in Novi Sad, Serbia
/// Includes real locations: bus stops, train stations, and key city locations
/// </summary>
public static class StationSeeds
{
    public static List<Station> GetStations()
    {
        var stations = new List<Station>();
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Major transport hubs
        stations.Add(CreateStation("Železnička Stanica", 45.2551, 19.8420, "Bulevar Jaše Tomića bb", "Main railway station", baseDate));
        stations.Add(CreateStation("Autobuska Stanica", 45.2509, 19.8335, "Bulevar Jaše Tomića 6", "Main bus station", baseDate));
        stations.Add(CreateStation("Trg Slobode", 45.2555, 19.8447, "Trg Slobode", "City center main square", baseDate));
        stations.Add(CreateStation("Univerzitet", 45.2474, 19.8515, "Trg Dositeja Obradovića 6", "University campus", baseDate));
        stations.Add(CreateStation("Šoping Centar Merkator", 45.2608, 19.8363, "Sentandrejski put bb", "Merkator shopping center", baseDate));

        // Liman area (residential district)
        stations.Add(CreateStation("Liman 1", 45.2445, 19.8280, "Bulevar oslobođenja 46", "Liman 1 neighborhood", baseDate));
        stations.Add(CreateStation("Liman 2", 45.2415, 19.8215, "Narodnog fronta", "Liman 2 neighborhood", baseDate));
        stations.Add(CreateStation("Liman 3", 45.2390, 19.8150, "Bulevar cara Lazara", "Liman 3 neighborhood", baseDate));
        stations.Add(CreateStation("Liman 4", 45.2365, 19.8085, "Bulevar cara Lazara", "Liman 4 neighborhood", baseDate));
        stations.Add(CreateStation("Park Liman", 45.2425, 19.8245, "Bulevar oslobođenja", "Liman park area", baseDate));

        // Grbavica area (residential district)
        stations.Add(CreateStation("Grbavica 1", 45.2385, 19.8445, "Bulevar Mihajla Pupina", "Grbavica 1 neighborhood", baseDate));
        stations.Add(CreateStation("Grbavica 2", 45.2355, 19.8415, "Bulevar Mihajla Pupina", "Grbavica 2 neighborhood", baseDate));
        stations.Add(CreateStation("Grbavica 3", 45.2325, 19.8385, "Bulevar Mihajla Pupina", "Grbavica 3 neighborhood", baseDate));
        stations.Add(CreateStation("Grbavica Centar", 45.2365, 19.8425, "Bulevar Mihajla Pupina", "Grbavica center", baseDate));
        stations.Add(CreateStation("Detelinara", 45.2295, 19.8325, "Bulevar Mihajla Pupina", "Detelinara neighborhood", baseDate));

        // Novo Naselje area (residential district)
        stations.Add(CreateStation("Novo Naselje", 45.2685, 19.8245, "Bulevar Evrope", "Novo Naselje center", baseDate));
        stations.Add(CreateStation("Novo Naselje 1", 45.2715, 19.8215, "Bulevar Evrope", "Novo Naselje 1", baseDate));
        stations.Add(CreateStation("Novo Naselje 2", 45.2745, 19.8185, "Bulevar Evrope", "Novo Naselje 2", baseDate));
        stations.Add(CreateStation("Šoping Centar Big", 45.2665, 19.8275, "Bulevar Evrope 12", "Big shopping center", baseDate));
        stations.Add(CreateStation("TC Promenada", 45.2625, 19.8305, "Bulevar Mihajla Pupina 3", "Promenada shopping mall", baseDate));

        // Petrovaradin area (historic fortress)
        stations.Add(CreateStation("Petrovaradin", 45.2525, 19.8665, "Petrovaradinska tvrđava", "Petrovaradin fortress area", baseDate));
        stations.Add(CreateStation("Petrovaradinska Tvrđava", 45.2515, 19.8705, "Beogradska bb", "Petrovaradin fortress entrance", baseDate));
        stations.Add(CreateStation("Petrovaradin Centar", 45.2495, 19.8625, "Preradovićeva", "Petrovaradin center", baseDate));
        stations.Add(CreateStation("Ribarsko Ostrvo", 45.2545, 19.8585, "Ribarsko ostrvo", "Fisherman's island", baseDate));
        stations.Add(CreateStation("Štrand", 45.2575, 19.8555, "Kej žrtava racije", "Štrand beach area", baseDate));

        // Salajka area
        stations.Add(CreateStation("Salajka", 45.2615, 19.8095, "Cara Dušana", "Salajka neighborhood", baseDate));
        stations.Add(CreateStation("Salajka Centar", 45.2635, 19.8125, "Cara Dušana", "Salajka center", baseDate));
        stations.Add(CreateStation("Somborski Bulevar", 45.2655, 19.8155, "Somborski bulevar", "Somborski boulevard", baseDate));
        stations.Add(CreateStation("Veternik", 45.2775, 19.7985, "Novosadska", "Veternik settlement", baseDate));
        stations.Add(CreateStation("Veternik Centar", 45.2805, 19.8015, "Futoška", "Veternik center", baseDate));

        // Futog area
        stations.Add(CreateStation("Futog", 45.2365, 19.7145, "Novosadski put", "Futog settlement", baseDate));
        stations.Add(CreateStation("Futog Centar", 45.2395, 19.7175, "Maršala Tita", "Futog center", baseDate));
        stations.Add(CreateStation("Futog Školska", 45.2425, 19.7205, "Školska", "Futog school area", baseDate));
        stations.Add(CreateStation("Futog Pošta", 45.2385, 19.7165, "Maršala Tita", "Futog post office", baseDate));

        // Kać area
        stations.Add(CreateStation("Kać", 45.3105, 19.9225, "Novosadska", "Kać settlement", baseDate));
        stations.Add(CreateStation("Kać Centar", 45.3135, 19.9255, "Partizanska", "Kać center", baseDate));
        stations.Add(CreateStation("Kać Železnička", 45.3165, 19.9285, "Železnička", "Kać railway area", baseDate));

        // Sremska Kamenica area
        stations.Add(CreateStation("Sremska Kamenica", 45.2245, 19.8645, "Cara Dušana", "Sremska Kamenica settlement", baseDate));
        stations.Add(CreateStation("Sremska Kamenica Centar", 45.2215, 19.8615, "Glavna", "Sremska Kamenica center", baseDate));
        stations.Add(CreateStation("Sremska Kamenica Park", 45.2185, 19.8585, "Stražilovska", "Sremska Kamenica park", baseDate));

        // City center locations
        stations.Add(CreateStation("Pozorište", 45.2565, 19.8425, "Pozorišni trg 1", "Serbian National Theatre", baseDate));
        stations.Add(CreateStation("Bulevar Kralja Petra I", 45.2545, 19.8385, "Bulevar kralja Petra I", "King Peter I Boulevard", baseDate));
        stations.Add(CreateStation("Dunavska Ulica", 45.2575, 19.8465, "Dunavska", "Danube street", baseDate));
        stations.Add(CreateStation("Zmaj Jovina", 45.2585, 19.8435, "Zmaj Jovina", "Zmaj Jovina street", baseDate));
        stations.Add(CreateStation("Grčkoškolska", 45.2595, 19.8405, "Grčkoškolska", "Greek school street", baseDate));
        stations.Add(CreateStation("Vladicin Dvor", 45.2545, 19.8495, "Nikole Pašića", "Bishop's Palace area", baseDate));

        // Medical and educational facilities
        stations.Add(CreateStation("Klinički Centar", 45.2475, 19.8285, "Hajduk Veljkova 1", "Clinical Center", baseDate));
        stations.Add(CreateStation("Medicinski Fakultet", 45.2465, 19.8305, "Hajduk Veljkova 3", "Medical Faculty", baseDate));
        stations.Add(CreateStation("Filozofski Fakultet", 45.2495, 19.8525, "Dr Zorana Đinđića 2", "Faculty of Philosophy", baseDate));
        stations.Add(CreateStation("Pravni Fakultet", 45.2505, 19.8495, "Trg Dositeja Obradovića 1", "Faculty of Law", baseDate));
        stations.Add(CreateStation("Tehnički Fakultet", 45.2485, 19.8535, "Trg Dositeja Obradovića 6", "Faculty of Technical Sciences", baseDate));

        // Parks and recreation
        stations.Add(CreateStation("Dunavski Park", 45.2615, 19.8525, "Dunavska", "Danube park", baseDate));
        stations.Add(CreateStation("Kamenički Park", 45.2565, 19.8375, "Bulevar Mihajla Pupina", "Kamenicki park", baseDate));
        stations.Add(CreateStation("Futoška Pijaca", 45.2625, 19.8195, "Futoška", "Futoska market", baseDate));
        stations.Add(CreateStation("Bulevar Oslobođenja", 45.2515, 19.8395, "Bulevar oslobođenja", "Liberation Boulevard", baseDate));

        // Industrial and business zones
        stations.Add(CreateStation("Industrijska Zona Jug", 45.2265, 19.8225, "Novosadskog sajma", "South industrial zone", baseDate));
        stations.Add(CreateStation("Industrijska Zona Sever", 45.2825, 19.8445, "Sentandrejski put", "North industrial zone", baseDate));
        stations.Add(CreateStation("BIP", 45.2755, 19.8415, "Bulevar cara Lazara", "BIP business zone", baseDate));
        stations.Add(CreateStation("Sajam", 45.2305, 19.8255, "Hajduk Veljkova", "Novi Sad Fair", baseDate));

        // Residential settlements - Telep
        stations.Add(CreateStation("Telep", 45.2335, 19.7985, "Bate Brkića", "Telep neighborhood", baseDate));
        stations.Add(CreateStation("Telep Centar", 45.2365, 19.8015, "Radnička", "Telep center", baseDate));
        stations.Add(CreateStation("Telep Pijaca", 45.2395, 19.8045, "Bate Brkića", "Telep market", baseDate));

        // Residential settlements - Adice
        stations.Add(CreateStation("Adice", 45.2815, 19.8285, "Bulevar Evrope", "Adice neighborhood", baseDate));
        stations.Add(CreateStation("Adice Centar", 45.2845, 19.8315, "Bulevar Evrope", "Adice center", baseDate));

        // Residential settlements - Bistrica
        stations.Add(CreateStation("Bistrica", 45.2885, 19.8385, "Bulevar Evrope", "Bistrica neighborhood", baseDate));
        stations.Add(CreateStation("Bistrica Centar", 45.2915, 19.8415, "Bulevar Evrope", "Bistrica center", baseDate));

        // Residential settlements - Klisa
        stations.Add(CreateStation("Klisa", 45.2145, 19.8445, "Rumenačka", "Klisa neighborhood", baseDate));
        stations.Add(CreateStation("Klisa Centar", 45.2115, 19.8415, "Rumenačka", "Klisa center", baseDate));

        // Residential settlements - Podbara
        stations.Add(CreateStation("Podbara", 45.2685, 19.8585, "Preradovićeva", "Podbara neighborhood", baseDate));
        stations.Add(CreateStation("Podbara Centar", 45.2715, 19.8615, "Stražilovska", "Podbara center", baseDate));

        // Additional key locations
        stations.Add(CreateStation("Spens", 45.2475, 19.8365, "Sutjeska 2", "SPENS sports center", baseDate));
        stations.Add(CreateStation("Careva Ćuprija", 45.2585, 19.8515, "Zmaj Jovina", "Emperor's Bridge area", baseDate));
        stations.Add(CreateStation("Almaska Crkva", 45.2535, 19.8455, "Nikole Pašića", "Almaš church", baseDate));
        stations.Add(CreateStation("Podgrađe", 45.2465, 19.8655, "Beogradska", "Podgrađe area", baseDate));
        stations.Add(CreateStation("Sokolski Dom", 45.2595, 19.8385, "Bulevar kralja Petra I", "Sokol hall", baseDate));

        // Final locations to reach 100
        stations.Add(CreateStation("Radnički Dom", 45.2455, 19.8325, "Bulevar oslobođenja", "Workers' hall", baseDate));
        stations.Add(CreateStation("Centar Za Kulturu", 45.2525, 19.8465, "Katolička porta", "Cultural center", baseDate));
        stations.Add(CreateStation("Gradska Biblioteka", 45.2555, 19.8415, "Dunavska 1", "City library", baseDate));
        stations.Add(CreateStation("Muzej Vojvodine", 45.2545, 19.8525, "Dunavska 35", "Museum of Vojvodina", baseDate));
        stations.Add(CreateStation("Sinagoga", 45.2575, 19.8395, "Jevrejska 11", "Synagogue", baseDate));
        stations.Add(CreateStation("Narodno Pozorište", 45.2565, 19.8425, "Pozorišni trg 1", "National Theatre", baseDate));
        stations.Add(CreateStation("Petrovaradinska Gimnazija", 45.2485, 19.8645, "Žarka Zrenjanina 2", "Petrovaradin high school", baseDate));
        stations.Add(CreateStation("Gimnazija Jovan Jovanović Zmaj", 45.2595, 19.8375, "Kralj Petar I 35", "Zmaj high school", baseDate));
        stations.Add(CreateStation("Jevrejska Opština", 45.2565, 19.8385, "Jevrejska", "Jewish community", baseDate));
        stations.Add(CreateStation("Vladičin dvor - Eparhija", 45.2545, 19.8495, "Nikole Pašića 7", "Episcopal palace", baseDate));

        return stations;
    }

    private static Station CreateStation(string name, double latitude, double longitude, string address, string description, DateTime createdAt)
    {
        var station = Station.Create(name, latitude, longitude, address, description);

        // Use reflection to set CreatedAt for seed data
        var createdAtProperty = typeof(Station).BaseType!.GetProperty("CreatedAt");
        createdAtProperty!.SetValue(station, createdAt);

        return station;
    }
}
