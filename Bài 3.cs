/*
    Trần Quốc Hùng_202418909
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementOOP
{
    public class Employee
    {
        // Thuộc tính (đóng gói private, cung cấp Getter public)
        public string Id { get; private set; }
        public string FullName { get; private set; }
        public double BaseSalary { get; private set; }

        // Constructor 1: Không tham số (dùng giá trị mặc định thông qua gọi Constructor 3)
        public Employee() : this("UNKNOWN", "Unnamed employee", 0) { }

        // Constructor 2: 2 tham số (lương mặc định = 0)
        public Employee(string id, string fullName) : this(id, fullName, 0) { }

        // Constructor 3: Đầy đủ 3 tham số + Kiểm tra ràng buộc (Bất biến)
        public Employee(string id, string fullName, double baseSalary)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Mã nhân sự không được rỗng!");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Họ tên không được rỗng!");
            if (baseSalary < 0)
                throw new ArgumentException("Lương cơ bản không được âm!");

            Id = id;
            FullName = fullName;
            BaseSalary = baseSalary;
        }

        // Nạp chồng 1: Tăng lương theo số tiền cố định
        public void increaseSalary(double amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("  [Lỗi] Giá trị tăng lương phải lớn hơn 0!");
                return;
            }
            BaseSalary += amount;
        }

        // Nạp chồng 2: Tăng lương theo phần trăm hoặc số tiền cố định
        public void increaseSalary(double value, bool byPercentage)
        {
            if (value <= 0)
            {
                Console.WriteLine("  [Lỗi] Giá trị tăng lương phải lớn hơn 0!");
                return;
            }

            if (byPercentage)
                BaseSalary += BaseSalary * (value / 100.0);
            else
                BaseSalary += value;
        }

        // Phương thức đa hình: Chi phí hàng tháng mặc định bằng lương cơ bản
        public virtual double calculateMonthlyCost()
        {
            return BaseSalary;
        }

        // Phương thức đa hình: Hiển thị thông tin
        public virtual void displayInfo()
        {
            Console.WriteLine($"  [Employee] Mã: {Id} | Tên: {FullName} | Lương CB: {BaseSalary:N0} | Chi phí tháng: {calculateMonthlyCost():N0}");
        }

        // Destructor quan sát đối tượng
        ~Employee()
        {
            Console.WriteLine($"  [~Destructor] Đối tượng Employee ({Id} - {FullName}) đã bị hủy.");
        }
    }

    public class SoftwareEngineer : Employee
    {
        // Thuộc tính bổ sung
        public string PrimaryLanguage { get; private set; }
        public double TechnicalAllowance { get; private set; }

        // Constructor 1
        public SoftwareEngineer(string id, string fullName, string primaryLanguage)
            : this(id, fullName, 0, primaryLanguage, 0) { }

        // Constructor 2
        public SoftwareEngineer(string id, string fullName, double baseSalary, string primaryLanguage, double technicalAllowance)
            : base(id, fullName, baseSalary)
        {
            if (string.IsNullOrWhiteSpace(primaryLanguage))
                throw new ArgumentException("Ngôn ngữ lập trình chính không được rỗng!");
            if (technicalAllowance < 0)
                throw new ArgumentException("Phụ cấp kỹ thuật không được âm!");

            PrimaryLanguage = primaryLanguage;
            TechnicalAllowance = technicalAllowance;
        }

        // Ghi đè: Tổng lương cơ bản + phụ cấp kỹ thuật
        public override double calculateMonthlyCost()
        {
            return BaseSalary + TechnicalAllowance;
        }

        // Ghi đè: Hiển thị thông tin kỹ sư phần mềm
        public override void displayInfo()
        {
            Console.WriteLine($"  [SoftwareEngineer] Mã: {Id} | Tên: {FullName} | Ngôn ngữ: {PrimaryLanguage} | Lương CB: {BaseSalary:N0} | Phụ cấp: {TechnicalAllowance:N0} | Chi phí tháng: {calculateMonthlyCost():N0}");
        }

        // Destructor
        ~SoftwareEngineer()
        {
            Console.WriteLine($"  [~Destructor] Đối tượng SoftwareEngineer ({Id} - {FullName}) đã bị hủy.");
        }
    }

    public class ProjectTeam : IDisposable
    {
        public string ProjectCode { get; private set; }
        public string ProjectName { get; private set; }

        // Liên kết không sở hữu (tham chiếu) đến trưởng nhóm và danh sách thành viên
        private Employee leader;
        private List<Employee> members;

        // Constructor 1: Tạo nhóm chưa có trưởng nhóm
        public ProjectTeam(string projectCode, string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectCode) || string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("Mã dự án và tên dự án không được rỗng!");

            ProjectCode = projectCode;
            ProjectName = projectName;
            leader = null;
            members = new List<Employee>();
        }

        // Constructor 2: Tạo nhóm có sẵn trưởng nhóm và tự đưa vào danh sách thành viên
        public ProjectTeam(string projectCode, string projectName, Employee leader)
            : this(projectCode, projectName)
        {
            if (leader == null)
                throw new ArgumentNullException("Trưởng nhóm không được null!");

            addMember(leader, true);
        }

        // Kiểm tra mã nhân sự đã có trong nhóm chưa
        public bool contains(string employeeId)
        {
            foreach (Employee emp in members)
            {
                if (emp.Id == employeeId)
                    return true;
            }
            return false;
        }

        // Nạp chồng addMember 1: Thêm thành viên bình thường
        public bool addMember(Employee employee)
        {
            return addMember(employee, false);
        }

        // Nạp chồng addMember 2: Thêm thành viên và tùy chọn đặt làm trưởng nhóm
        public bool addMember(Employee employee, bool makeLeader)
        {
            if (employee == null) return false;

            // Kiểm tra trùng lặp
            if (contains(employee.Id))
            {
                Console.WriteLine($"  [Từ chối] Nhân sự '{employee.FullName}' (Mã: {employee.Id}) đã tồn tại trong nhóm {ProjectCode}!");
                return false;
            }

            members.Add(employee);

            // Nếu makeLeader == true, đặt làm trưởng nhóm (trưởng nhóm cũ vẫn nằm trong members)
            if (makeLeader)
            {
                leader = employee;
            }

            return true;
        }

        // Đổi trưởng nhóm
        public void changeLeader(Employee employee)
        {
            if (employee == null) return;

            // Nếu trưởng nhóm mới chưa phải thành viên thì tự động thêm vào nhóm
            if (!contains(employee.Id))
            {
                members.Add(employee);
            }

            leader = employee;
            Console.WriteLine($"  [Thành công] Đã đổi trưởng nhóm {ProjectCode} thành: {employee.FullName} ({employee.Id}).");
        }

        // Xóa thành viên theo mã
        public bool removeMember(string employeeId)
        {
            // Ràng buộc
            if (leader != null && leader.Id == employeeId)
            {
                Console.WriteLine($"  [Từ chối] Không thể xóa '{leader.FullName}' ({employeeId}) vì đang là Trưởng nhóm! Hãy đổi trưởng nhóm trước.");
                return false;
            }

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].Id == employeeId)
                {
                    Console.WriteLine($"  [Thành công] Đã xóa nhân sự mã {employeeId} khỏi nhóm {ProjectCode}.");
                    members.RemoveAt(i);
                    return true;
                }
            }

            Console.WriteLine($"  [Lỗi] Không tìm thấy nhân sự mã {employeeId} trong nhóm {ProjectCode}.");
            return false;
        }

        // Tính tổng chi phí nhân sự hằng tháng
        public double calculateTotalMonthlyCost()
        {
            double total = 0;
            foreach (Employee emp in members)
            {
                total += emp.calculateMonthlyCost();
            }
            return total;
        }

        // Hiển thị thông tin nhóm
        public void displayTeam()
        {
            Console.WriteLine($"\n--- THÔNG TIN NHÓM DỰ ÁN: [{ProjectCode}] {ProjectName} ---");
            if (leader != null)
                Console.WriteLine($"  * Trưởng nhóm: {leader.FullName} (Mã: {leader.Id})");
            else
                Console.WriteLine("  * Trưởng nhóm: (Chưa có)");

            Console.WriteLine($"  * Danh sách thành viên ({members.Count} người):");
            foreach (Employee emp in members)
            {
                emp.displayInfo(); // Lời gọi đa hình!
            }
        }

        // Hủy cấu trúc danh sách nội bộ khi kết thúc khối lệnh cục bộ (không hủy các đối tượng Employee)
        public void Dispose()
        {
            members.Clear();
            leader = null;
            Console.WriteLine($"  [~Destructor ProjectTeam] Đã hủy danh sách nội bộ của nhóm [{ProjectCode}]. Các đối tượng Employee vẫn tồn tại độc lập!");
            GC.SuppressFinalize(this);
        }

        ~ProjectTeam()
        {
            Console.WriteLine($"  [~Destructor ProjectTeam] Nhóm [{ProjectCode}] đã bị hủy.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("================ BẮT ĐẦU KIỂM THỬ KỊCH BẢN ================\n");

            //Tạo hai Employee bằng hai constructor khác nhau
            Console.WriteLine("1. Tạo 2 Employee bằng 2 constructor khác nhau:");
            Employee emp1 = new Employee("E01", "Nguyễn Văn An"); // Lương mặc định = 0
            Employee emp2 = new Employee("E02", "Trần Thị Bình", 10000000);
            emp1.displayInfo();
            emp2.displayInfo();

            //Tạo hai SoftwareEngineer bằng hai constructor khác nhau
            Console.WriteLine("\n2. Tạo 2 SoftwareEngineer bằng 2 constructor khác nhau:");
            SoftwareEngineer se1 = new SoftwareEngineer("SE01", "Lê Hoàng Nam", "C#");
            SoftwareEngineer se2 = new SoftwareEngineer("SE02", "Phạm Minh Tuấn", 20000000, "C++", 5000000);
            se1.displayInfo();
            se2.displayInfo();

            //Tăng lương một nhân sự bằng số tiền cố định
            Console.WriteLine("\n3. Tăng lương cố định cho E01 thêm 12,000,000:");
            emp1.increaseSalary(12000000);
            emp1.displayInfo();

            //Tăng lương một nhân sự khác theo phần trăm
            Console.WriteLine("\n4. Tăng lương theo phần trăm (10%) cho SE02:");
            se2.increaseSalary(10, true);
            se2.displayInfo();

            //Tạo nhóm dự án không có trưởng nhóm
            Console.WriteLine("\n5. Tạo nhóm dự án PRJ01 chưa có trưởng nhóm.");
            ProjectTeam team1 = new ProjectTeam("PRJ01", "Hệ thống Quản lý Ngân hàng");

            //Thêm một nhân sự vào nhóm bằng addMember(employee)
            Console.WriteLine("\n6. Thêm E01 và E02 vào nhóm PRJ01:");
            team1.addMember(emp1);
            team1.addMember(emp2);

            //Thêm một kỹ sư bằng addMember(employee, true) để đặt làm trưởng nhóm
            Console.WriteLine("\n7. Thêm kỹ sư SE02 vào nhóm và đặt làm trưởng nhóm:");
            team1.addMember(se2, true);

            //Thử thêm lại một thành viên đã tồn tại
            Console.WriteLine("\n8. Thử thêm lại E01 (đã tồn tại trong nhóm):");
            team1.addMember(emp1);

            //Hiển thị danh sách bằng lời gọi đa hình
            Console.WriteLine("\n9. Hiển thị danh sách nhóm bằng lời gọi đa hình:");
            team1.displayTeam();

            //Tính tổng chi phí nhân sự hằng tháng
            Console.WriteLine($"\n10. Tổng chi phí nhân sự hằng tháng của PRJ01: {team1.calculateTotalMonthlyCost():N0} VNĐ");

            //Thử xóa trưởng nhóm hiện tại và kiểm tra thao tác bị từ chối
            Console.WriteLine("\n11. Thử xóa trưởng nhóm hiện tại (SE02):");
            team1.removeMember("SE02");

            //Đổi trưởng nhóm rồi xóa người từng là trưởng nhóm
            Console.WriteLine("\n12. Đổi trưởng nhóm sang SE01 rồi xóa trưởng nhóm cũ (SE02):");
            team1.changeLeader(se1);
            team1.removeMember("SE02");
            team1.displayTeam();

            //Tạo nhóm thứ hai trong một khối lệnh cục bộ, thêm nhân sự từ nhóm 1, rồi hủy nhóm 2
            Console.WriteLine("\n13 & 14. Tạo nhóm thứ 2 (PRJ02) trong khối lệnh cục bộ và thêm E01 (đang ở PRJ01):");
            {
                using (ProjectTeam team2 = new ProjectTeam("PRJ02", "Ứng dụng AI", emp1))
                {
                    team2.displayTeam();
                    Console.WriteLine("\n-> Kết thúc khối lệnh cục bộ của PRJ02:");
                } // Tự động gọi Dispose() hủy danh sách nội bộ của team2 ngay tại đây
            }

            //Chứng minh nhân sự của nhóm thứ hai vẫn tồn tại sau khi nhóm bị hủy
            Console.WriteLine("\n15. Chứng minh nhân sự E01 vẫn tồn tại độc lập sau khi PRJ02 bị hủy:");
            emp1.displayInfo();
        }
    }
}