//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;

namespace MaterialUI
{
    public class DropdownInstantiationHelper : InstantiationHelper
    {
        [SerializeField]
        private MaterialButton m_Button;

        public override void HelpInstantiate(params InstantiationOptions[] options)
        {
            if (!options.Contains(InstantiationOptions.Raised))
            {
                m_Button.Convert(true);
            }

            base.HelpInstantiate(options);
        }
    }
}