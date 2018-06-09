using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{
    class LRU
    {
        private int cursor;
        public int p_frame_size;
        public List<Page> frame_window;
        public List<Page> pageHistory;

        public int hit;
        public int fault;
        public int migration;

        public LRU(int get_frame_size)
        {
            this.cursor = 0;
            this.p_frame_size = get_frame_size;
            this.frame_window = new List<Page>();
            this.pageHistory = new List<Page>();
        }

        public Page.STATUS Operate(char data, ref int[] count, ref int cnt, ref int[] numb, ref int nu)
        {
            Page newPage;

            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }

                for (int n = 0; n < p_frame_size; n++)
                    count[n]++;
                count[i] = 0;
                newPage.loc = i + 1;

                nu++;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;

                    int max = count[0];
                    int maxloc = 0;
                    for (int n = 1; n < p_frame_size; n++)
                    {
                        if (count[n] > max)
                        {
                            max = count[n];
                            maxloc = n;
                        }
                    }

                    this.frame_window.RemoveAt(maxloc);
                    for (int n = 0; n < p_frame_size; n++)
                        count[n]++;
                    count[maxloc] = 0;
                    cursor = maxloc + 1;
                    this.migration++;
                    this.fault++;

                    newPage.loc = cursor;
                    frame_window.Insert(maxloc, newPage);

                    numb[nu] = cursor - 1;
                    nu++;
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    cursor++;
                    this.fault++;
                    for (int n = 0; n < p_frame_size; n++)
                    {
                        if (count[n] != 0)
                            count[n]++;
                    }
                    count[cnt] = 1;
                    cnt++;

                    newPage.loc = cursor;
                    frame_window.Add(newPage);

                    nu++;
                }

            }
            pageHistory.Add(newPage);

            return newPage.status;
        }

        public List<Page> GetPageInfo(Page.STATUS status)
        {
            List<Page> pages = new List<Page>();

            foreach (Page page in pageHistory)
            {
                if (page.status == status)
                {
                    pages.Add(page);
                }
            }

            return pages;
        }

    }
   
}