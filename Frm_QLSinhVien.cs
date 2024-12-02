using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLySinhVien.Entities;

namespace QuanLySinhVien
{
    public partial class Frm_DanhSachSinhVien : Form
    {
        List<Khoa> lstKhoa;
        List<SinhVien> lstSinhVien = new List<SinhVien>();
        List<string> lstRank;
        public Frm_DanhSachSinhVien()
        {
            InitializeComponent();
            cmbKhoa.DisplayMember = "TenKhoa";
            cmbKhoa.ValueMember = "MaKhoa";
        }

        public void SetGridViewStyle(DataGridView dgview)
        {
            // Loại bỏ viền của DataGridView để tạo cảm giác nhẹ nhàng hơn
            dgview.BorderStyle = BorderStyle.None;

            // Đặt màu nền khi chọn dòng là màu DarkTurquoise để nổi bật lựa chọn
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;

            // Sử dụng viền đơn giữa các ô, tạo cảm giác thanh thoát và đơn giản
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Đặt màu nền chính của DataGridView là màu trắng, tạo độ tương phản cao với các hàng dữ liệu
            dgview.BackgroundColor = Color.White;

            // Đặt chế độ chọn toàn bộ hàng khi người dùng nhấp vào một ô, cải thiện trải nghiệm sử dụng
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Frm_DanhSachSinhVien_Load(object sender, EventArgs e)
        {
            SetGridViewStyle(dgvDSSV);
            lstKhoa = new List<Khoa>()
            {
                new Khoa() {MaKhoa = "CNTT", TenKhoa = "Công nghệ thông tin"},
                new Khoa() {MaKhoa = "QTKD", TenKhoa = "Quản trị kinh doanh"},
                new Khoa() {MaKhoa = "NNA", TenKhoa = "Ngôn ngữ Anh"},
            };
            cmbKhoa.DataSource = lstKhoa;

            lstRank = new List<string>(){ "Tăng dần", "Giảm dần"};
            cmbRank.DataSource = lstRank;
            cmbRank.SelectedIndex = -1;

            //khoa QTKD và giới tính nữ được chọn mặc định  
            cmbKhoa.SelectedValue = "QTKD";
            rbWomen.Checked = true;


            txtMenCount.Text = txtWomenCount.Text = "0";

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //kiểm tra dữ liệu nhập vào có hợp lệ không
            if (string.IsNullOrEmpty(txtID.Text) || 
                string.IsNullOrEmpty(txtName.Text) || 
                string.IsNullOrEmpty(txtAVG.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtID.Focus();
                return;
            }

            //kiểm tra mã số sinh viên có đúng định dạng một chuỗi số gồm 10 ký tự không
            if (txtID.Text.Length != 10 || !txtID.Text.All(char.IsDigit))
            {
                MessageBox.Show("Mã số sinh viên không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtID.Focus();
                return;
            }

            //kiểm tra điểm trung bình có nằm trong khoảng 0-10 không
            if (double.Parse(txtAVG.Text) < 0 || double.Parse(txtAVG.Text) > 10)
            {
                MessageBox.Show("Điểm trung bình sinh viên không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAVG.Focus();
                return;
            }

       

            //lấy dữ liệu từ txt
            string maSinhVien = txtID.Text;
            string tenSinhVien = txtName.Text;
            double avgScore = double.Parse(txtAVG.Text);
            string maKhoa = cmbKhoa.SelectedValue.ToString();
            string gioiTinh = rbMen.Checked ? "Nam" : "Nữ";

            //kiểm tra sinh viên tồn tại trong danh sách chưa
            SinhVien sv = lstSinhVien.FirstOrDefault(s => s.MaSV == maSinhVien);
            if (sv == null)
            {
                // tạo mới sinh viên
                sv = new SinhVien(maSinhVien, tenSinhVien, avgScore, maKhoa, gioiTinh);
                lstSinhVien.Add(sv);
                MessageBox.Show("Thêm mới dữ liệu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //cập nhật sinh viên
                sv.Ten = tenSinhVien;
                sv.Diem = avgScore;
                sv.MaKhoa = maKhoa;
                sv.GioiTinh = gioiTinh;
                MessageBox.Show("Cập nhật dữ liệu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            //hiển thị danh sách sinh viên lên dgvQLSinhVIen
            HienThiDanhSachSinhVien();

            //hiển thị thống kê số lượng sinh viên nam, nữ
            txtMenCount.Text = lstSinhVien.Count(s => s.GioiTinh == "Nam").ToString();
            txtWomenCount.Text = lstSinhVien.Count(s => s.GioiTinh == "Nữ").ToString();

            //reset form 
            txtID.Text = txtName.Text = txtAVG.Text = "";
            cmbKhoa.SelectedValue = "QTKD";
            rbWomen.Checked = true;
        }

        private void UpdateRank()
        {
            var sortedList =  lstSinhVien.OrderByDescending(s => s.Diem).ToList();

            // Gán thứ hạng cho từng sinh viên
            for (int i = 0; i < sortedList.Count; i++)
            {
                sortedList[i].rank = i + 1; // Thứ hạng bắt đầu từ 1
            }
        }

        private void HienThiDanhSachSinhVien()
        {
            //xóa dữ liệu cũ trên dataGridView
            dgvDSSV.Rows.Clear();
            //cập nhật thứ hạng cho từng sinh viên
            UpdateRank();
            //hiển thị dữ liệu mới lên dataGridView
            foreach (SinhVien sv in lstSinhVien)
            {
                int index = dgvDSSV.Rows.Add();
                dgvDSSV.Rows[index].Cells[0].Value = sv.MaSV;
                dgvDSSV.Rows[index].Cells[1].Value = sv.Ten;
                dgvDSSV.Rows[index].Cells[2].Value = sv.GioiTinh;
                dgvDSSV.Rows[index].Cells[3].Value = sv.Diem;
                string tenKhoa = lstKhoa.FirstOrDefault(s => s.MaKhoa == sv.MaKhoa).TenKhoa;
                dgvDSSV.Rows[index].Cells[4].Value = tenKhoa;
                dgvDSSV.Rows[index].Cells[5].Value = sv.rank;
            } 
        }

        private void dgvDSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //kiểm tra người dùng có click vào dòng header không
            if (e.RowIndex < 0) return;
            //lấy thông tin sinh viên từ dòng đc click
            string maSinhVien = dgvDSSV.Rows[e.RowIndex].Cells[0].Value.ToString();
            SinhVien sv = lstSinhVien.FirstOrDefault(s => s.MaSV == maSinhVien);
            if (sv != null)
            {
                //hiển thị thông tin sinh viên lên form
                txtID.Text = sv.MaSV;
                txtName.Text = sv.Ten;
                txtAVG.Text = sv.Diem.ToString();
                cmbKhoa.SelectedValue = sv.MaKhoa;
                rbMen.Checked = sv.GioiTinh == "Nam";
                rbWomen.Checked = sv.GioiTinh == "Nữ";
            }
        }

        private void Frm_DanhSachSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Kiểm tra người dùng có muốn thoát chương trình không
            DialogResult result = MessageBox.Show("Bạn có muốn thoát chương trình không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            //không cho nhập số và kí tự đặc biệt
            if (char.IsDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar) || char.IsSymbol(e.KeyChar))
            {
                errName.SetError(txtName, "Tên sinh viên không được chứa số hoặc kí tự đặc biệt");
                e.Handled = true;
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //kiểm tra mssv có trong danh sách hay không
            string maSinhVien = txtID.Text;
            SinhVien sv = lstSinhVien.FirstOrDefault(s => s.MaSV == maSinhVien);
            if (sv != null) {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                //xóa sinh viên khỏi danh sách
                lstSinhVien.Remove(sv);
                MessageBox.Show("Xóa dữ liệu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //hiển thị lại danh sách sinh viên
                HienThiDanhSachSinhVien();
                //hiển thị lại thống kê số lượng sinh viên nam, nữ
                txtMenCount.Text = lstSinhVien.Count(s => s.GioiTinh == "Nam").ToString();
                txtWomenCount.Text = lstSinhVien.Count(s => s.GioiTinh == "Nữ").ToString();
                //reset form
                txtID.Text = txtName.Text = txtAVG.Text = "";
                cmbKhoa.SelectedValue = "QTKD";
                rbWomen.Checked = true;
            }
            else
            {
                MessageBox.Show("Mã số sinh viên không tồn tại trong hệ thống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //lọc danh sách sinh viên theo mã số sinh viên hoặc tên sau đó hiển thị lên DataGridView
            string keyword = txtSearch.Text;
            if(keyword == "")
            {
                HienThiDanhSachSinhVien();
                return;
            }
            List<SinhVien> lstSearch = lstSinhVien.Where(s => s.MaSV.Contains(keyword) || s.Ten.Contains(keyword)).ToList();
            dgvDSSV.Rows.Clear();
            foreach (SinhVien sv in lstSearch)
            {
                int index = dgvDSSV.Rows.Add();
                dgvDSSV.Rows[index].Cells[0].Value = sv.MaSV;
                dgvDSSV.Rows[index].Cells[1].Value = sv.Ten;
                dgvDSSV.Rows[index].Cells[2].Value = sv.GioiTinh;
                dgvDSSV.Rows[index].Cells[3].Value = sv.Diem;
                string tenKhoa = lstKhoa.FirstOrDefault(s => s.MaKhoa == sv.MaKhoa).TenKhoa;
                dgvDSSV.Rows[index].Cells[4].Value = tenKhoa;
            }

        }

        private void cmbRank_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbRank.SelectedIndex == 0)
            {
                //hiển thị danh sách sắp xếp tăng dần theo rank
                dgvDSSV.Sort(dgvDSSV.Columns[5], ListSortDirection.Ascending);
            }
               
            else if(cmbRank.SelectedIndex == 1)
            {
                //hiển thị danh sách sắp xếp giảm dần theo rank
                dgvDSSV.Sort(dgvDSSV.Columns[5], ListSortDirection.Descending);
            }
        }

        
    }
}
