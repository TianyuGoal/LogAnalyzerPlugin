using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LogAnalyzerPlugin.Core;

namespace LogAnalyzerPlugin.Forms
{
    public partial class MainForm : Form
    {
        private TextBox txtKeywords;
        private NumericUpDown numContextLines;
        private NumericUpDown numCooccurrenceWindow;
        private CheckBox chkCaseSensitive;
        private Button btnSearch;
        private TabControl tabResults;
        private RichTextBox txtContextResults;
        private RichTextBox txtCooccurrenceResults;
        private RichTextBox txtTimelineResults;
        private Label lblStatus;

        private string currentText = "";
        private LogAnalyzer analyzer;

        public MainForm()
        {
            InitializeComponent();
            analyzer = new LogAnalyzer(3, 10);
        }

        private void InitializeComponent()
        {
            this.Text = "日志分析器 - 多关键字搜索";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 顶部控制面板
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 150, Padding = new Padding(10) };
            this.Controls.Add(panelTop);

            // 关键字输入
            var lblKeywords = new Label { Text = "关键字（每行一个）:", Location = new Point(10, 10), AutoSize = true };
            panelTop.Controls.Add(lblKeywords);

            txtKeywords = new TextBox
            {
                Location = new Point(10, 35),
                Size = new Size(250, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9)
            };
            panelTop.Controls.Add(txtKeywords);

            // 配置选项
            var lblContext = new Label { Text = "上下文行数:", Location = new Point(280, 10), AutoSize = true };
            panelTop.Controls.Add(lblContext);
            numContextLines = new NumericUpDown
            {
                Location = new Point(280, 35),
                Width = 80,
                Minimum = 0,
                Maximum = 20,
                Value = 3
            };
            panelTop.Controls.Add(numContextLines);

            var lblWindow = new Label { Text = "共现窗口:", Location = new Point(380, 10), AutoSize = true };
            panelTop.Controls.Add(lblWindow);
            numCooccurrenceWindow = new NumericUpDown
            {
                Location = new Point(380, 35),
                Width = 80,
                Minimum = 2,
                Maximum = 50,
                Value = 10
            };
            panelTop.Controls.Add(numCooccurrenceWindow);

            chkCaseSensitive = new CheckBox
            {
                Text = "区分大小写",
                Location = new Point(280, 70),
                AutoSize = true
            };
            panelTop.Controls.Add(chkCaseSensitive);

            // 搜索按钮
            btnSearch = new Button
            {
                Text = "搜索分析",
                Location = new Point(280, 100),
                Size = new Size(180, 35),
                Font = new Font("微软雅黑", 10, FontStyle.Bold)
            };
            btnSearch.Click += BtnSearch_Click;
            panelTop.Controls.Add(btnSearch);

            // 状态栏
            lblStatus = new Label
            {
                Text = "就绪",
                Dock = DockStyle.Bottom,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                BackColor = Color.LightGray
            };
            this.Controls.Add(lblStatus);

            // 结果标签页
            tabResults = new TabControl { Dock = DockStyle.Fill };
            this.Controls.Add(tabResults);

            // Tab 1: 上下文结果
            var tabContext = new TabPage("上下文结果");
            txtContextResults = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                WordWrap = false
            };
            tabContext.Controls.Add(txtContextResults);
            tabResults.TabPages.Add(tabContext);

            // Tab 2: 共现块
            var tabCooccurrence = new TabPage("关键字共现");
            txtCooccurrenceResults = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                WordWrap = false
            };
            tabCooccurrence.Controls.Add(txtCooccurrenceResults);
            tabResults.TabPages.Add(tabCooccurrence);

            // Tab 3: 时间线
            var tabTimeline = new TabPage("时间线视图");
            txtTimelineResults = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                WordWrap = false
            };
            tabTimeline.Controls.Add(txtTimelineResults);
            tabResults.TabPages.Add(tabTimeline);
        }

        public void SetText(string text)
        {
            currentText = text;
            lblStatus.Text = $"已加载文本，共 {text.Split('\n').Length} 行";
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var keywords = txtKeywords.Lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .ToList();

            if (keywords.Count == 0)
            {
                MessageBox.Show("请输入至少一个关键字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(currentText))
            {
                MessageBox.Show("当前文档为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblStatus.Text = "搜索中...";
            Application.DoEvents();

            try
            {
                analyzer = new LogAnalyzer((int)numContextLines.Value, (int)numCooccurrenceWindow.Value);
                bool caseSensitive = chkCaseSensitive.Checked;

                // 1. 上下文搜索
                var results = analyzer.SearchKeywords(currentText, keywords, caseSensitive);
                DisplayContextResults(results);

                // 2. 共现分析
                var cooccurrences = analyzer.FindCoOccurrences(currentText, keywords, caseSensitive);
                DisplayCooccurrenceResults(cooccurrences);

                // 3. 时间线
                var timeline = analyzer.BuildTimeline(currentText, keywords, caseSensitive);
                DisplayTimelineResults(timeline);

                lblStatus.Text = $"完成：找到 {results.Count} 个匹配，{cooccurrences.Count} 个共现块，{timeline.Count} 个时间线事件";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"搜索出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "搜索失败";
            }
        }

        private void DisplayContextResults(List<SearchResult> results)
        {
            txtContextResults.Clear();
            if (results.Count == 0)
            {
                txtContextResults.Text = "未找到匹配结果";
                return;
            }

            foreach (var r in results)
            {
                txtContextResults.AppendText($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
                txtContextResults.AppendText($"关键字: [{r.Keyword}]  行号: {r.LineNumber}");
                if (r.Timestamp != null)
                    txtContextResults.AppendText($"  时间: {r.Timestamp:yyyy-MM-dd HH:mm:ss}");
                txtContextResults.AppendText("\n\n");

                if (r.ContextBefore.Count > 0)
                {
                    txtContextResults.AppendText("【上文】\n");
                    foreach (var line in r.ContextBefore)
                        txtContextResults.AppendText(line + "\n");
                }

                txtContextResults.AppendText($">>> {r.LineNumber,6}: {r.LineContent}\n");

                if (r.ContextAfter.Count > 0)
                {
                    txtContextResults.AppendText("【下文】\n");
                    foreach (var line in r.ContextAfter)
                        txtContextResults.AppendText(line + "\n");
                }

                txtContextResults.AppendText("\n");
            }
        }

        private void DisplayCooccurrenceResults(List<CoOccurrenceBlock> blocks)
        {
            txtCooccurrenceResults.Clear();
            if (blocks.Count == 0)
            {
                txtCooccurrenceResults.Text = "未发现关键字共现";
                return;
            }

            txtCooccurrenceResults.AppendText($"共发现 {blocks.Count} 个关键字共现块\n\n");

            foreach (var block in blocks)
            {
                txtCooccurrenceResults.AppendText($"╔═══════════════════════════════════════════════════════\n");
                txtCooccurrenceResults.AppendText($"║ 共现关键字: {string.Join(", ", block.MatchedKeywords)}\n");
                txtCooccurrenceResults.AppendText($"║ 行范围: {block.StartLine} - {block.EndLine}");
                if (block.EarliestTimestamp != null)
                    txtCooccurrenceResults.AppendText($"  时间: {block.EarliestTimestamp:yyyy-MM-dd HH:mm:ss}");
                txtCooccurrenceResults.AppendText("\n╚═══════════════════════════════════════════════════════\n");

                foreach (var line in block.Lines)
                    txtCooccurrenceResults.AppendText(line + "\n");

                txtCooccurrenceResults.AppendText("\n");
            }
        }

        private void DisplayTimelineResults(List<TimelineEvent> events)
        {
            txtTimelineResults.Clear();
            if (events.Count == 0)
            {
                txtTimelineResults.Text = "未检测到时间戳信息（日志需包含标准时间格式）";
                return;
            }

            txtTimelineResults.AppendText($"时间线事件序列（共 {events.Count} 个事件）\n");
            txtTimelineResults.AppendText("═══════════════════════════════════════════════════════════════\n\n");

            foreach (var evt in events)
            {
                txtTimelineResults.AppendText($"{evt.Timestamp:yyyy-MM-dd HH:mm:ss.fff}  ");
                txtTimelineResults.AppendText($"[{evt.Keyword}]  ");
                txtTimelineResults.AppendText($"行{evt.LineNumber}  ");
                txtTimelineResults.AppendText($"{evt.LineContent}\n");
            }
        }
    }
}
