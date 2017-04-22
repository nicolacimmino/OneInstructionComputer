using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneInstructionComputer
{
    /// <summary>
    /// Imlements a lookup table in which items of two different types function
    ///     as both key and value.
    ///     
    /// An usage example could be to map color names to color values allowing to both
    ///     lookup the value for a certain color name or the name for a certain color
    ///     value.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    class BidirectionalLookup<TLeft, TRight>
    {
        // A dictionary where right elements are arranged by left keys
        Dictionary<TLeft, TRight> leftToRight = new Dictionary<TLeft, TRight>();

        // A dictionary where left elements are arranged by right keys
        Dictionary<TRight, TLeft> rightToLeft = new Dictionary<TRight, TLeft>();

        /// <summary>
        /// Add an element to the lookup
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void Add(TLeft left, TRight right)
        {
            if (leftToRight.ContainsKey(left))
            {
                throw new ArgumentException("Left is not unique.");
            }
            if (rightToLeft.ContainsKey(right))
            {
                throw new ArgumentException("Right is not unique.");
            }

            leftToRight.Add(left, right);
            rightToLeft.Add(right, left);
        }

        /// <summary>
        /// Get a left element knowng the right key
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public TLeft this[TRight right]
        {
            get 
            {
                return rightToLeft[right];
            }
            set { /* set the specified index to value here */ }
        }

        /// <summary>
        /// Get a right element knowing the left key
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public TRight this[TLeft left]
        {
            get
            {
                return leftToRight[left];
            }
            set { /* set the specified index to value here */ }
        }

        /// <summary>
        /// Clear all elements.
        /// </summary>
        public void Clear()
        {
            leftToRight.Clear();
            rightToLeft.Clear();
        }

        /// <summary>
        /// Gets all left values.
        /// </summary>
        /// <returns></returns>
        public List<TLeft> ValuesLeft()
        {
            return rightToLeft.Values.ToList<TLeft>();
        }

        /// <summary>
        /// Gets all right values.
        /// </summary>
        /// <returns></returns>
        public List<TRight> ValuesRight()
        {
            return leftToRight.Values.ToList<TRight>();
        }
 
    }
}
