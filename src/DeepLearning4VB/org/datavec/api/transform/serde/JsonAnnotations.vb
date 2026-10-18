'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System

' The attributes below stand in for the Jackson annotations of the Java DataVec classes. They are read by
' JsonMappers (see there for the complete set of rules) and have no effect with plain System.Text.Json options.
Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' Values of this type, and of every class implementing or extending it, are written with an <c>"@class"</c>
    ''' property holding the Java class name of the runtime type, and are read back through that name
    ''' (Java: <c>@JsonTypeInfo(use = JsonTypeInfo.Id.CLASS, include = JsonTypeInfo.As.PROPERTY, property = "@class")</c>).
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface, Inherited:=False, AllowMultiple:=False)>
    Public NotInheritable Class JsonClassTypeAttribute
        Inherits Attribute
    End Class

    ''' <summary>
    ''' Values of this type are written as a wrapper object whose single property is named after the runtime type:
    ''' <c>{"Reducer" : { ... }}</c> (Java: <c>@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include =
    ''' JsonTypeInfo.As.WRAPPER_OBJECT)</c> with <c>@JsonSubTypes</c>). Apply it once per subtype name; an implementation
    ''' without a listed name uses its simple class name, as Jackson does.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface, Inherited:=False, AllowMultiple:=True)>
    Public NotInheritable Class JsonWrapperTypeAttribute
        Inherits Attribute

        Private ReadOnly subTypeValue As Type
        Private ReadOnly nameValue As String

        ''' <param name="subType"> An implementation of the annotated type </param>
        ''' <param name="name">    The wrapper property name used for it </param>
        Public Sub New(ByVal subType As Type, ByVal name As String)
            Me.subTypeValue = subType
            Me.nameValue = name
        End Sub

        ''' <summary>The implementation this name applies to.</summary>
        Public ReadOnly Property SubType As Type
            Get
                Return subTypeValue
            End Get
        End Property

        ''' <summary>The wrapper property name.</summary>
        Public ReadOnly Property Name As String
            Get
                Return nameValue
            End Get
        End Property
    End Class

    ''' <summary>
    ''' JSON property names (Java field names) that are neither written nor read for this class
    ''' (Java: <c>@JsonIgnoreProperties({...})</c>). As in Jackson, the attribute of the most derived class wins: a
    ''' subclass that declares its own list replaces the list of its base class.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class, Inherited:=True, AllowMultiple:=False)>
    Public NotInheritable Class JsonIgnorePropertiesAttribute
        Inherits Attribute

        Private ReadOnly namesValue As String()

        Public Sub New(ByVal ParamArray names() As String)
            namesValue = If(names, New String() {})
        End Sub

        ''' <summary>The ignored JSON property names.</summary>
        Public ReadOnly Property Names As String()
            Get
                Return namesValue
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Null-valued fields of this type (and of its subtypes and implementations) are not written
    ''' (Java: <c>@JsonInclude(JsonInclude.Include.NON_NULL)</c>). Without it, nulls are written as <c>null</c>.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface, Inherited:=True, AllowMultiple:=False)>
    Public NotInheritable Class JsonIncludeNonNullAttribute
        Inherits Attribute
    End Class

    ''' <summary>
    ''' The JSON property name bound to a parameter of the constructor marked with <c>&lt;JsonConstructor&gt;</c>, when
    ''' it differs from the parameter name (Java: <c>@JsonProperty("name")</c> on a creator parameter).
    ''' </summary>
    <AttributeUsage(AttributeTargets.Parameter, Inherited:=False, AllowMultiple:=False)>
    Public NotInheritable Class JsonPropertyAttribute
        Inherits Attribute

        Private ReadOnly nameValue As String

        Public Sub New(ByVal name As String)
            Me.nameValue = name
        End Sub

        ''' <summary>The JSON property name.</summary>
        Public ReadOnly Property Name As String
            Get
                Return nameValue
            End Get
        End Property
    End Class

End Namespace
